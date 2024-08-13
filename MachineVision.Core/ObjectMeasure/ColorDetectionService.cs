using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch;
using Prism.Mvvm;

namespace MachineVision.Core.ObjectMeasure
{
    public class ColorDetectionService : BindableBase
    {
        public ColorDetectionService()
        {
            info = new MethodInfo()
            {
                Name = "trans_from_rgb",
                Description = "Transform an image from the RGB color space to an arbitrary color space.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter(){Name="ImageRed",Description="输入图像（红色通道）"},
                    new MethodParmeter(){Name="ImageGreen",Description="输入图像（绿色通道）"},
                    new MethodParmeter(){Name="ImageBlue",Description="输入图像（蓝色通道）"},
                    new MethodParmeter(){Name="ImageResult1",Description="颜色转换的输出图像（通道1）"},
                    new MethodParmeter(){Name="ImageResult2",Description="颜色转换的输出图像（通道1）"},
                    new MethodParmeter(){Name="ImageResult3",Description="颜色转换的输出图像（通道1）"},
                    new MethodParmeter(){Name="ColorSpace",Description="输出图像的颜色空间"},
                },
                Predecessors = new List<string>()
                {
                    "decompose3"
                }
            };
        }
        public MethodInfo info { get; set; }
        public HObject RoiObject { get; set; }

        HObject ho_Red, ho_Green, ho_Blue;
        HObject ho_Hue, ho_Saturation, ho_Saturated, ho_Intensity, imageReduced;
        HObject ho_HueSaturated, ho_CurrentFuse = null, ho_CurrentFuseConn = null;
        HObject ho_CurrentFuseFill = null, ho_CurrentFuseSel = null;


        HTuple hv_FuseColors = new HTuple(), hv_HueRanges = new HTuple();
        HTuple hv_Fuse = new HTuple(), hv_FuseArea = new HTuple();
        HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
        HTuple hv_i = new HTuple();

        private HWindow hWindow;

        public HWindow HWindow
        {
            get { return hWindow; }
            set { hWindow = value; RaisePropertyChanged(); }
        }

        public string Run(HObject image)
        {
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_Hue);
            HOperatorSet.GenEmptyObj(out ho_Saturation);
            HOperatorSet.GenEmptyObj(out ho_Intensity);
            HOperatorSet.GenEmptyObj(out ho_Saturated);
            HOperatorSet.GenEmptyObj(out ho_HueSaturated);
            HOperatorSet.GenEmptyObj(out ho_CurrentFuse);
            HOperatorSet.GenEmptyObj(out ho_CurrentFuseConn);
            HOperatorSet.GenEmptyObj(out ho_CurrentFuseFill);
            HOperatorSet.GenEmptyObj(out ho_CurrentFuseSel);
            HOperatorSet.GenEmptyObj(out imageReduced);

            hv_FuseColors = new HTuple();
            hv_FuseColors[0] = "Orange";
            hv_FuseColors[1] = "Red";
            hv_FuseColors[2] = "Blue";
            hv_FuseColors[3] = "Yellow";
            hv_FuseColors[4] = "Green";

            hv_HueRanges = new HTuple();
            hv_HueRanges[0] = 10;
            hv_HueRanges[1] = 30;
            hv_HueRanges[2] = 0;
            hv_HueRanges[3] = 10;
            hv_HueRanges[4] = 125;
            hv_HueRanges[5] = 162;
            hv_HueRanges[6] = 30;
            hv_HueRanges[7] = 64;
            hv_HueRanges[8] = 96;
            hv_HueRanges[9] = 128;

            try
            {
                var timeSpan = SetTimeHepler.SetTimer(() =>
                {
                    if (RoiObject != null)
                    {
                        HOperatorSet.ReduceDomain(image, (HObject)RoiObject, out imageReduced);
                    }
                    else
                    {
                        imageReduced = image;
                    }
                    HOperatorSet.Decompose3(imageReduced, out ho_Red, out ho_Green, out ho_Blue);
                    HOperatorSet.TransFromRgb(ho_Red, ho_Green, ho_Blue, out ho_Hue, out ho_Saturation, out ho_Intensity, "hsv");
                    HOperatorSet.Threshold(ho_Saturation, out ho_Saturated, 60, 255);
                    HOperatorSet.ReduceDomain(ho_Hue, ho_Saturated, out ho_HueSaturated);

                    for (hv_Fuse = 0; (int)hv_Fuse <= (int)((new HTuple(hv_FuseColors.TupleLength())) - 1); hv_Fuse = (int)hv_Fuse + 1)
                    {
                        HOperatorSet.Threshold(ho_HueSaturated, out ho_CurrentFuse, hv_HueRanges.TupleSelect(hv_Fuse * 2), hv_HueRanges.TupleSelect((hv_Fuse * 2) + 1));
                        HOperatorSet.Connection(ho_CurrentFuse, out ho_CurrentFuseConn);
                        HOperatorSet.FillUp(ho_CurrentFuseConn, out ho_CurrentFuseFill);
                        HOperatorSet.SelectShape(ho_CurrentFuseFill, out ho_CurrentFuseSel, "area", "and", 6000, 20000);
                        HOperatorSet.AreaCenter(ho_CurrentFuseSel, out hv_FuseArea, out hv_Row1, out hv_Column1);
                        HOperatorSet.SetColor(HWindow, "magenta");

                        for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_FuseArea.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                        {
                            HOperatorSet.SetTposition(HWindow, hv_Row1.TupleSelect(hv_i), hv_Column1.TupleSelect(hv_i));
                            HOperatorSet.WriteString(HWindow, hv_FuseColors.TupleSelect(hv_Fuse));
                        }
                        HOperatorSet.SetTposition(HWindow, 24 * (hv_Fuse + 1), 12);
                        HOperatorSet.SetColor(HWindow, "slate blue");
                        HOperatorSet.WriteString(HWindow, ((hv_FuseColors.TupleSelect(hv_Fuse)) + " Fuses: ") + (new HTuple(hv_FuseArea.TupleLength())));
                    }
                });
                return $"颜色检测成功! 检测耗时:{DateTime.Now}:{timeSpan}";
            }
            catch (Exception ex)
            {
                return $"颜色检测失败! {DateTime.Now}:{ex.Message}";
            }
        }
    }
}
