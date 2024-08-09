using HalconDotNet;
using MachineVision.Core.TemplateMatch;
using Prism.Mvvm;

namespace MachineVision.Core.ObjectMeasure
{
    public class CircleMeasureService : BindableBase
    {
        public CircleMeasureService()
        {
            info = new MethodInfo()
            {
                Name = "add_metrology_object_circle_measure",
                Description = "Add a circle or a circular arc to a metrology model.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter(){Name="MetrologyHandle",Description="计量模型的处理"},
                    new MethodParmeter(){Name="Row",Description="圆或圆弧中心的行坐标（或Y）"},
                    new MethodParmeter(){Name="Column",Description="圆或圆弧中心的列（或X）坐标"},
                    new MethodParmeter(){Name="Radius",Description="圆或圆弧的半径"},
                    new MethodParmeter(){Name="MeasureLength1",Description="垂直于边界的测量区域的一半长度"},
                    new MethodParmeter(){Name="MeasureLength2",Description="与边界相切的测量区域的一半长度"},
                    new MethodParmeter(){Name="MeasureSigma",Description="用于平滑的高斯函数的Sigma"},
                    new MethodParmeter(){Name="MeasureThreshold",Description="最小边缘幅度"},
                    new MethodParmeter(){Name="GenParamName",Description="泛型参数的名称"},
                    new MethodParmeter(){Name="GenParamValue",Description="泛型参数的值"},
                    new MethodParmeter(){Name="Index",Description="创建的计量对象的索引"},
                },
                Predecessors = new List<string>()
                {
                    "set_metrology_model_image_size"
                }
            };
            RunParameter = new MeasureRunParameter();
            RunParameter.ApplyDefaultParameter();
        }

        private HWindow hWindow;

        public HWindow HWindow
        {
            get { return hWindow; }
            set { hWindow = value;RaisePropertyChanged(); }
        }


        public MethodInfo info { get; set; }
        private MeasureRunParameter runParameter;
        public MeasureRunParameter RunParameter { get { return runParameter; } set { runParameter = value; RaisePropertyChanged(); } }

        HTuple hv_MetrologyHandle = new HTuple(), hv_Index = new HTuple();
        HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
        public string Run(HObject image)
        {
            if(image == null) { return "图片异常!"; }
            if(RunParameter.Row==0 || RunParameter.Column==0 || RunParameter.Radius == 0) { return "参数异常,请先绘制圆并提取圆参数!"; }
            HObject ho_GrayImage, ho_Contour, ho_Contours;
            HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
            HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle,
                RunParameter.Row,
                RunParameter.Column,
                RunParameter.Radius,
                RunParameter.MeasureLength1,
                RunParameter.MeasureLength2,
                RunParameter.MeasureSigma,
                RunParameter.MeasureThreshold, new HTuple(), new HTuple(), out hv_Index);
            HOperatorSet.Rgb1ToGray(image, out ho_GrayImage);
            HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row1, out hv_Column1);
            HOperatorSet.ApplyMetrologyModel(ho_GrayImage, hv_MetrologyHandle);
            HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, 0, "all", "result_type", "all_param", out HTuple hv_Parameter);
            HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, 0, "all", 1.5);

            if(HWindow != null)
            {
                HOperatorSet.SetColor(HWindow, "red");
                HWindow.DispObj(ho_Contours);
                HOperatorSet.SetColor(HWindow, "blue");
                HWindow.DispObj(ho_Contour);
            }

            try
            {
                return $"坐标:({hv_Parameter.DArr[0]},{hv_Parameter.DArr[1]},半径:{hv_Parameter.DArr[2]})";
            }
            catch
            {
                return $"查找圆失败";
            }
            
        }
    }

    public class MeasureRunParameter : BaseParameter
    {
        private double row;

        public double Row
        {
            get { return row; }
            set { row = value; RaisePropertyChanged(); }
        }

        private double column;

        public double Column
        {
            get { return column; }
            set { column = value; RaisePropertyChanged(); }
        }

        private double radius;

        public double Radius
        {
            get { return radius; }
            set { radius = value; RaisePropertyChanged(); }
        }

        private double measureLength1;

        public double MeasureLength1
        {
            get { return measureLength1; }
            set { measureLength1 = value; RaisePropertyChanged(); }
        }

        private double measureLength2;

        public double MeasureLength2
        {
            get { return measureLength2; }
            set { measureLength2 = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 平滑系数
        /// </summary>
        private int measureSigma;

        public int MeasureSigma
        {
            get { return measureSigma; }
            set { measureSigma = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 边缘阈值
        /// </summary>
        private int measureThreshold;

        public int MeasureThreshold
        {
            get { return measureThreshold; }
            set { measureThreshold = value; RaisePropertyChanged(); }
        }

        public override void ApplyDefaultParameter()
        {
            MeasureSigma = 1;
            MeasureThreshold = 30;
            MeasureLength1 = 20;
            MeasureLength2 = 5;
        }
    }

}
