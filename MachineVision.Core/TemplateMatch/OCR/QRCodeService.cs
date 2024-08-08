using HalconDotNet;
using MachineVision.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.TemplateMatch.OCR
{
    public class QRCodeService
    {
        public QRCodeService()
        {
            info = new MethodInfo()
            {
                Name = "find_data_code_2d",
                Description = "Detect and read 2D data code symbols in an image or train the 2D data code model.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter() { Name = "Image", Description = "输入图像" },
                    new MethodParmeter() { Name = "SymbolXLDs ", Description = "围绕成功解码的数据代码符号的XLD轮廓" },
                    new MethodParmeter() { Name = "DataCodeHandle", Description = "二维数据代码模型的句柄" },
                    new MethodParmeter() { Name = "GenParamNames", Description = "用于控制操作符行为的（可选）参数的名称" },
                    new MethodParmeter() { Name = "GenParamValues", Description = "可选泛型参数的值" },
                    new MethodParmeter() { Name = "ResultHandles", Description = "处理所有成功解码的2D数据代码符号" },
                    new MethodParmeter() { Name = "DecodedDataStrings", Description = "图像中所有检测到的2D数据代码符号的解码数据串" },
                },
                Predecessors = new List<string>()
                {
                    "create_data_code_2d_model",
                    "read_data_code_2d_model",
                    "set_data_code_2d_param"
                }
            };

            HOperatorSet.CreateDataCode2dModel("QR Code", new HTuple(), new HTuple(), out hv_DataCodeHandle);
        }

        public MethodInfo info { get; set; }
        public HObject RoiObject { get; set; }

        HObject ho_SymbolXLDs = null;
        HTuple hv_ImageFiles = new HTuple(), hv_ImageNum = new HTuple();
        HTuple hv_Message = new HTuple();
        HTuple hv_DataCodeHandle = new HTuple(), hv_Index = new HTuple();
        HTuple hv_ResultHandles = new HTuple(), hv_DecodedDataStrings = new HTuple();

        public OcrMatchResult Run(HObject image)
        {
            double timespan = SetTimeHepler.SetTimer(() =>
            {
                HOperatorSet.FindDataCode2d(image, out ho_SymbolXLDs, hv_DataCodeHandle, new HTuple(), new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);
            });
            if (!string.IsNullOrWhiteSpace(hv_DecodedDataStrings))
            {
                return new OcrMatchResult
                {
                    IsSuccess = true,
                    Message = $"{DateTime.Now} 匹配耗时:{timespan} ms,匹配结果:{hv_DecodedDataStrings}",
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
