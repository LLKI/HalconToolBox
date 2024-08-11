using HalconDotNet;
using MachineVision.Core.Extensions;
using Prism.Mvvm;

namespace MachineVision.Core.TemplateMatch.OCR
{
    public class CharRecognitionService: BindableBase
    {
        public CharRecognitionService()
        {
            info = new MethodInfo()
            {
                Name = "find_text",
                Description = "Find text in an image.",
                Parmeters = new List<MethodParmeter>()
                    {
                        new MethodParmeter() { Name = "Image", Description = "输入图像" },
                        new MethodParmeter() { Name = "TextModel", Description = "文本模型指定要分割的文本" },
                        new MethodParmeter() { Name = "TextResultID", Description = "分割的结果" },
                    },
                Predecessors = new List<string>()
                    {
                        "create_text_model_reader",
                        "set_text_model_param",
                        "text_line_orientation",
                        "text_line_slant",
                    }
            };

            HOperatorSet.CreateTextModelReader("auto", "Universal_Rej.occ", out hv_TextModel);
        }

        public MethodInfo info { get; set; }
        public HObject RoiObject { get; set; }

        HTuple hv_TextModel = new HTuple();
        HTuple hv_TextResultID = new HTuple(), hv_ResultValue = new HTuple();

        private HWindow hWindow;

        public HWindow HWindow
        {
            get { return hWindow; }
            set { hWindow = value; RaisePropertyChanged(); }
        }

        public OcrMatchResult Run(HObject image)
        {

            HObject imageReduced, ho_Characters;
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out ho_Characters);

            double timespan = SetTimeHepler.SetTimer(() =>
            {

                if (RoiObject != null)
                {
                    HOperatorSet.ReduceDomain(image, (HObject)RoiObject, out imageReduced);
                    RoiObject = null;//用完就清空
                }
                else
                {
                    imageReduced = image;
                }

                //查找文本
                //只是获取TextResultID文本结果ID
                hv_TextResultID.Dispose();
                HOperatorSet.FindText(imageReduced, hv_TextModel, out hv_TextResultID);
                //获取文本内容
                HOperatorSet.GetTextResult(hv_TextResultID, "class", out hv_ResultValue);
                //获取文本区域
                HOperatorSet.GetTextObject(out ho_Characters, hv_TextResultID, "all_lines");
            });

            if (HWindow != null)
            {
                HOperatorSet.SetColor(HWindow, "red");
                HWindow.DispObj(ho_Characters);
            }

            if (!string.IsNullOrWhiteSpace(hv_ResultValue))
            {
                return new OcrMatchResult()
                {
                    IsSuccess = true,
                    Message = $"{DateTime.Now} 识别耗时:{timespan} ms,识别结果:{hv_ResultValue}",
                    TimeSpan = timespan
                };
            }
            else
            {
                return new OcrMatchResult() { IsSuccess = false, Message = $"{DateTime.Now} 识别耗时:{timespan} ms,识别失败" };
            }
        }
    }
}
