using HalconDotNet;
using MachineVision.Core.Extensions;
namespace MachineVision.Core.TemplateMatch.OCR
{
    public class BarCodeService
    {
        public BarCodeService()
        {
            info = new MethodInfo()
            {
                Name = "find_bar_code ",
                Description = "Detect and read bar code symbols in an image.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter() { Name = "Image", Description = "输入图像" },
                    new MethodParmeter() { Name = "SymbolRegions", Description = "成功解码的条形码符号的区域" },
                    new MethodParmeter() { Name = "BarCodeHandle", Description = "条形码模型的句柄" },
                    new MethodParmeter() { Name = "CodeType", Description = "搜索的条形码的类型" },
                    new MethodParmeter() { Name = "DecodedDataStrings", Description = "所有成功解码的条形码的数据串" },
                },
                Predecessors = new List<string>()
                {
                    "create_bar_code_model",
                    "set_bar_code _model",
                }
            };

            HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
        }

        public MethodInfo info { get; set; }
        public HObject RoiObject { get; set; }

        HObject ho_Image = null, ho_SymbolRegions = null;
        HTuple hv_BarCodeHandle = new HTuple();
        HTuple hv_I = new HTuple(), hv_DecodedDataStrings = new HTuple();
        HTuple hv_Reference = new HTuple(), hv_String = new HTuple();
        HTuple hv_J = new HTuple(), hv_Char = new HTuple();

        public OcrMatchResult Run(HObject image)
        {
            HObject imageReduced;
            HOperatorSet.GenEmptyObj(out imageReduced);
            if (RoiObject != null)
            {
                HOperatorSet.ReduceDomain(image, (HObject)RoiObject, out imageReduced);
                RoiObject = null;//用完就清空
            }
            else
            {
                imageReduced = image;
            }

            double timespan = SetTimeHepler.SetTimer(() =>
            {
                HOperatorSet.FindBarCode(imageReduced, out ho_SymbolRegions, hv_BarCodeHandle, "Code 128", out hv_DecodedDataStrings);
                if(hv_DecodedDataStrings.Length==0)
                {
                    return;
                }
                HOperatorSet.GetBarCodeResult(hv_BarCodeHandle, 0, "decoded_reference", out hv_Reference);
            });
            if (hv_DecodedDataStrings.Length == 0)
            {
                return new OcrMatchResult() { IsSuccess = false, Message = $"{DateTime.Now} 匹配耗时:{timespan} ms,匹配失败" };
            }
            hv_String = "";
            HTuple end_val15 = (hv_DecodedDataStrings.TupleStrlen()) - 1;
            HTuple step_val15 = 1;
            for (hv_J = 0; hv_J.Continue(end_val15, step_val15); hv_J = hv_J.TupleAdd(step_val15))
            {
                if ((int)(new HTuple(((((hv_DecodedDataStrings.TupleStrBitSelect(hv_J))).TupleOrd())).TupleLess(32))) != 0)
                {
                    hv_Char.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Char = "\\x" + (((((hv_DecodedDataStrings.TupleStrBitSelect(hv_J))).TupleOrd())).TupleString("02x"));
                    }
                }
                else
                {
                    hv_Char.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Char = hv_DecodedDataStrings.TupleStrBitSelect(hv_J);
                    }
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple ExpTmpLocalVar_String = hv_String + hv_Char;
                        hv_String.Dispose();
                        hv_String = ExpTmpLocalVar_String;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(hv_String))
            {
                return new OcrMatchResult()
                {
                    IsSuccess = true,
                    Message = $"{DateTime.Now} 匹配耗时:{timespan} ms,匹配结果:{hv_String}",
                    TimeSpan = timespan
                };
            }
            else
            {
                return new OcrMatchResult() { IsSuccess = false, Message = $"{DateTime.Now} 匹配耗时:{timespan} ms,匹配失败" };
            }
        }
    }
}
