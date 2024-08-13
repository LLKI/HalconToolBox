using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch;
using Prism.Mvvm;

namespace MachineVision.Core.ObjectMeasure
{
    public class SobelAmpService : BindableBase
    {
        public SobelAmpService()
        {
            info = new MethodInfo()
            {
                Name = "sobel_amp",
                Description = "Detect edges (amplitude) using the Sobel operator.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter(){Name="Image",Description="输入图像"},
                    new MethodParmeter(){Name="FilterType",Description="过滤器类型"},
                    new MethodParmeter(){Name="Size",Description="过滤器掩码的大小"},
                },
                Predecessors = new List<string>()
                {
                    "binomial_filter",
                    "gauss_filter",
                    "mean_image",
                    "anisotropic_diffusion",
                    "sigma_image"
                }
            };
        }
        public MethodInfo info { get; set; }
        public HObject RoiObject { get; set; }
        private HWindow hWindow;

        public HWindow HWindow
        {
            get { return hWindow; }
            set { hWindow = value; RaisePropertyChanged(); }
        }

        HObject ho_EdgeAmplitude, ho_Edges, ho_EdgesExtended, imageReduced;
        HTuple hv_Width = new HTuple(), hv_Height = new HTuple();

        public string Run(HObject image)
        {
            HOperatorSet.GenEmptyObj(out ho_EdgeAmplitude);
            HOperatorSet.GenEmptyObj(out ho_Edges);
            HOperatorSet.GenEmptyObj(out ho_EdgesExtended);
            HOperatorSet.GenEmptyObj(out imageReduced);
            if (RoiObject != null)
            {
                HOperatorSet.ReduceDomain(image, (HObject)RoiObject, out imageReduced);
            }
            else
            {
                imageReduced = image;
            }
            try
            {
                var timespan = SetTimeHepler.SetTimer(() =>
                {
                    HOperatorSet.SobelAmp(imageReduced, out ho_EdgeAmplitude, "thin_sum_abs", 3);
                    HOperatorSet.Threshold(ho_EdgeAmplitude, out ho_Edges, 30, 255);
                    HOperatorSet.CloseEdges(ho_Edges, ho_EdgeAmplitude, out ho_EdgesExtended, 15);
                    HOperatorSet.SetColor(HWindow, "green");
                    HWindow.DispObj(ho_EdgesExtended);
                    HOperatorSet.SetColor(HWindow, "red");
                    HWindow.DispObj(ho_Edges);
                });
                return $"检测边缘成功!耗时:{DateTime.Now}:{timespan}";
            }
            catch (Exception ex)
            {
                return $"检测边缘失败! {DateTime.Now}" +ex.Message;
            }
        }
    }
}
