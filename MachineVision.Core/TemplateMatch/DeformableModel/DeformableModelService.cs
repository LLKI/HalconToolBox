using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Prism.Mvvm;

namespace MachineVision.Core.TemplateMatch.DeformableModel
{
    public class DeformableModelService: BindableBase, ITemplateMatchService
    {
        public DeformableModelService()
        {
            info = new MethodInfo()
            {
                Name = "find_local_deformable_model",
                Description = "Find the best matches of a local deformable model in an image.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter(){Name="Image",Description="模型的输入图像"},
                    new MethodParmeter(){Name="ImageRectified",Description="找到的模型的校正图像"},
                    new MethodParmeter(){Name="VectorField",Description="矢量场的整流变换"},
                    new MethodParmeter(){Name="DeformedContours",Description="找到的模型实例的轮廓"},
                    new MethodParmeter(){Name="ModelID",Description="模型的句柄"},
                    new MethodParmeter(){Name="AngleStart",Description="模型的最小旋转"},
                    new MethodParmeter(){Name="AngleExtent",Description="旋转角度的范围"},
                    new MethodParmeter(){Name="ScaleRMin",Description="模型在行方向上的最小比例"},
                    new MethodParmeter(){Name="ScaleRMax",Description="模型在行方向上的最大比例"},
                    new MethodParmeter(){Name="ScaleCMin",Description="模型在列方向上的最小比例"},
                    new MethodParmeter(){Name="ScaleCMax",Description="模型在列方向上的最大比例"},
                    new MethodParmeter(){Name="MinScore",Description="要查找的模型实例的最小得分"},
                    new MethodParmeter(){Name="NumMatcher",Description="要查找的模型的实例数（0表示所有匹配项）"},
                    new MethodParmeter(){Name="MaxOverlap",Description="要查找的模型实例的最大重叠"},
                    new MethodParmeter(){Name="NumLevels",Description="匹配中使用的金字塔等级数"},
                    new MethodParmeter(){Name="Greediness",Description="搜索启发式的“贪婪”（0：安全但慢; 1：快，但可能会错过匹配）"},
                    new MethodParmeter(){Name="ResultType",Description="为请求的图标结果切换"},
                    new MethodParmeter(){Name="GenParamName",Description="常规参数名称"},
                    new MethodParmeter(){Name="GenParamValue",Description="一般参数的值"},
                    new MethodParmeter(){Name="Score",Description="找到的模型实例的得分"},
                    new MethodParmeter(){Name="Row",Description="找到的模型实例的行坐标"},
                    new MethodParmeter(){Name="Column",Description="找到的模型实例的列坐标"}
                },
                Predecessors = new List<string>()
                {
                    "create_local_deformable_model",
                    "create_local_deformable_model_xld",
                    "read_deformable_model"
                }
            };
            RunParameter = new DeformableModelRunParameter();
            TemplateParameter = new DeformableModelInputParameter();
            Setting = new MatchResultSetting();

            //设置默认参数
            RunParameter.ApplyDefaultParameter();
            TemplateParameter.ApplyDefaultParameter();
        }

        private MatchResultSetting setting;
        public HObject RoiObject { get; set; }
        public MethodInfo info { get; set; }
        private DeformableModelInputParameter templateParameter;
        private DeformableModelRunParameter runParameter;
        private HTuple modelId = new HTuple();
        HTuple hv_Row = new HTuple(), hv_Col = new HTuple(), hv_Score = new HTuple();
        HObject ho_VectorField, ho_DeformedContours, ho_ImageRectified;

        public MatchResultSetting Setting
        {
            get { return setting; }
            set { setting = value; RaisePropertyChanged(); }
        }

        public DeformableModelInputParameter TemplateParameter
        {
            get { return templateParameter; }
            set { templateParameter = value; RaisePropertyChanged(); }
        }

        public DeformableModelRunParameter RunParameter
        {
            get { return runParameter; }
            set { runParameter = value; RaisePropertyChanged(); }
        }
        public async Task CreateTemplate(HObject image, HObject hObject)
        {
            try
            {
                await Task.Run(() =>
                {
                    HObject template;
                    HOperatorSet.GenEmptyObj(out template);
                    HOperatorSet.ReduceDomain(image, hObject, out template);

                    HOperatorSet.CreateLocalDeformableModel(template,
                        TemplateParameter.NumLevels,
                        TemplateParameter.AngleStart,
                        TemplateParameter.AngleExtent,
                        TemplateParameter.AngleStep,
                        TemplateParameter.ScaleRMin,
                        TemplateParameter.ScaleRMax,
                        TemplateParameter.ScaleRStep,
                        TemplateParameter.ScaleCMin,
                        TemplateParameter.ScaleCMax,
                        TemplateParameter.ScaleCStep,
                        TemplateParameter.Optimization,
                        TemplateParameter.Metric,
                        TemplateParameter.Contrast,
                        TemplateParameter.MinContrast,
                        TemplateParameter.GenParamName,
                        TemplateParameter.GenParamValue,
                        out modelId);
                });
            }
            catch (Exception)
            {
                return;
            }
        }

        public MatchResult Run(HObject image)
        {
            MatchResult matchResult = new MatchResult();
            matchResult.Results = new ObservableCollection<TemplateMatchResult>();

            try
            {
                if (image == null)
                {
                    matchResult.Message = "输入图像无效!";
                    return matchResult;
                }
                if (modelId.Length == 0)
                {
                    matchResult.Message = "输入模板无效!";
                    return matchResult;
                }

                var timeSpan = SetTimer(() =>
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
                    HOperatorSet.FindLocalDeformableModel(
                                imageReduced,
                                out ho_ImageRectified,
                                out ho_VectorField,
                                out ho_DeformedContours,
                                modelId,
                                RunParameter.AngleStart,
                                RunParameter.AngleExtent,
                                RunParameter.ScaleRMin,
                                RunParameter.ScaleRMax,
                                RunParameter.ScaleCMin,
                                RunParameter.ScaleCMax,
                                RunParameter.MinScore,
                                RunParameter.NumMatches,
                                RunParameter.Maxoverlap,
                                RunParameter.NumLevels,
                                RunParameter.Greediness,
                                RunParameter.ResultType,
                                RunParameter.GenParamName == "[]" ? new HTuple() : RunParameter.GenParamName,
                                RunParameter.GenParamValue == "[]" ? new HTuple() : RunParameter.GenParamValue,
                                out hv_Score, out hv_Row, out hv_Col);
                });

                for (int i = 0; i < hv_Score.Length; i++)
                {
                    matchResult.Results.Add(new TemplateMatchResult()
                    {
                        Index = i + 1,
                        Row = hv_Row[i],
                        Column = hv_Col[i],
                        Score = hv_Score.DArr[i],
                        Contours = ho_DeformedContours
                    });
                }
                matchResult.TimeSpan = timeSpan;
                matchResult.Setting = Setting;
                matchResult.Message = $"匹配耗时:{timeSpan} ms , 匹配个数:{matchResult.Results.Count}";
                return matchResult;
            }
            catch (Exception ex)
            {
                matchResult.Message = $"匹配失败,"+ex.Message;
                return matchResult;
            }
           
        }

        private double SetTimer(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }


    /// <summary>
    /// 形变匹配模板参数
    /// </summary>
    public class DeformableModelInputParameter : BaseParameter
    {
        private string numLevels;

        private double angleStart;
        private double angleExtent;
        private string angleStep;

        private double scaleRMin;
        private double scaleRMax;
        private string scaleRStep;

        private int scaleCMin;
        private int scaleCMax;
        private string scaleCStep;

        private string optimization;
        private string metric;
        private string contrast;
        private string minContrast;

        private string genParamName;
        private string genParamValue;

        public string GenParamValue
        {
            get { return genParamValue; }
            set { genParamValue = value; RaisePropertyChanged(); }
        }
        public string GenParamName
        {
            get { return genParamName; }
            set { genParamName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 最小对比度
        /// </summary>
        public string MinContrast
        {
            get { return minContrast; }
            set { minContrast = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 对比度
        /// </summary>
        public string Contrast
        {
            get { return contrast; }
            set { contrast = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 匹配方法
        /// </summary>
        public string Metric
        {
            get { return metric; }
            set { metric = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 模板优化方法
        /// </summary>
        public string Optimization
        {
            get { return optimization; }
            set { optimization = value; RaisePropertyChanged(); }
        }

        public string ScaleCStep
        {
            get { return scaleCStep; }
            set { scaleCStep = value; RaisePropertyChanged(); }
        }

        public int ScaleCMax
        {
            get { return scaleCMax; }
            set { scaleCMax = value; RaisePropertyChanged(); }
        }

        public int ScaleCMin
        {
            get { return scaleCMin; }
            set { scaleCMin = value; RaisePropertyChanged(); }
        }


        public string ScaleRStep
        {
            get { return scaleRStep; }
            set { scaleRStep = value; RaisePropertyChanged(); }
        }


        public double ScaleRMax
        {
            get { return scaleRMax; }
            set { scaleRMax = value; RaisePropertyChanged(); }
        }


        public double ScaleRMin
        {
            get { return scaleRMin; }
            set { scaleRMin = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 旋转角度步长
        /// </summary>
        public string AngleStep
        {
            get { return angleStep; }
            set { angleStep = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 模板旋转角度范围
        /// </summary>
        public double AngleExtent
        {
            get { return angleExtent; }
            set { angleExtent = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 模板旋转起始角度
        /// </summary>
        public double AngleStart
        {
            get { return angleStart; }
            set { angleStart = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 金字塔层数
        /// </summary>
        public string NumLevels
        {
            get { return numLevels; }
            set { numLevels = value; RaisePropertyChanged(); }
        }

        public override void ApplyDefaultParameter()
        {
            NumLevels = "auto";

            AngleStart = 0;
            AngleExtent = 360;
            AngleStep = "auto";

            ScaleRMin = 1;
            ScaleRMax = 1;
            ScaleRStep = "auto";

            ScaleCMin = 1;
            ScaleCMax = 1;
            ScaleCStep = "auto";

            Optimization = "none";
            Metric = "use_polarity";

            Contrast = "auto";
            MinContrast = "auto";

            GenParamName = "part_size";
            GenParamValue = "medium";
        }

    }

    /// <summary>
    /// 形变匹配模板运行参数
    /// </summary>
    public class DeformableModelRunParameter : BaseParameter
    {
        private double angleStart;
        private double angleExtent;
        private double scaleRMin;
        private double scaleRMax;
        private double scaleCMin;
        private double scaleCMax;
        private double minScore;
        private int numMatches;
        private double maxOverlap;
        private int numLevels;
        private double greediness;
        private string resultType;
        private string genParamName;
        private string genParamValue;

        public string GenParamValue
        {
            get { return genParamValue; }
            set { genParamValue = value;RaisePropertyChanged(); }
        }


        public string GenParamName
        {
            get { return genParamName; }
            set { genParamName = value; RaisePropertyChanged(); }
        }


        public string ResultType
        {
            get { return resultType; }
            set { resultType = value; RaisePropertyChanged(); }
        }


        public double ScaleCMax
        {
            get { return scaleCMax; }
            set { scaleCMax = value; RaisePropertyChanged(); }
        }


        public double ScaleCMin
        {
            get { return scaleCMin; }
            set { scaleCMin = value; RaisePropertyChanged(); }
        }


        public double ScaleRMax
        {
            get { return scaleRMax; }
            set { scaleRMax = value; RaisePropertyChanged(); }
        }


        public double ScaleRMin
        {
            get { return scaleRMin; }
            set { scaleRMin = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 匹配个数
        /// </summary>
        public int NumMatches
        {
            get { return numMatches; }
            set { numMatches = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 重叠率
        /// </summary>
        public double Maxoverlap
        {
            get { return maxOverlap; }
            set { maxOverlap = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 贪婪程度
        /// </summary>
        public double Greediness
        {
            get { return greediness; }
            set { greediness = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 分数
        /// </summary>
        public double MinScore
        {
            get { return minScore; }
            set { minScore = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 模板旋转角度范围
        /// </summary>
        public double AngleExtent
        {
            get { return angleExtent; }
            set { angleExtent = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 模板旋转起始角度
        /// </summary>
        public double AngleStart
        {
            get { return angleStart; }
            set { angleStart = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 金字塔层数
        /// </summary>
        public int NumLevels
        {
            get { return numLevels; }
            set { numLevels = value; RaisePropertyChanged(); }
        }

        public override void ApplyDefaultParameter()
        {
            AngleStart = 0;
            AngleExtent = 360;
            ScaleRMin = 1;
            ScaleRMax = 1;
            ScaleCMin = 1;
            ScaleCMax = 1;
            MinScore = 0.5;
            NumMatches = 2;
            Maxoverlap = 1;
            NumLevels = 0;
            Greediness = 0.9;
            ResultType = "deformed_contours";
            GenParamName = "[]";
            GenParamValue = "[]";
        }
    }
}
