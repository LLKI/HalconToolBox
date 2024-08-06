using HalconDotNet;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace MachineVision.Core.TemplateMatch
{
    public class ShapeModelService : BindableBase, ITemplateMatchService
    {
        public ShapeModelService()
        {
            info = new MethodInfo()
            {
                Name = "find_shape_model",
                Description = "Find the best matches of a shape model in an image.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter(){Name="Image",Description="输入图像"},
                    new MethodParmeter(){Name="ModelID",Description="模型的句柄"},
                    new MethodParmeter(){Name="AngleStart",Description="模型的最小旋转"},
                    new MethodParmeter(){Name="AngleExtent",Description="旋转角度的范围"},
                    new MethodParmeter(){Name="MinScore",Description="要查找的模型实例的最小得分"},
                    new MethodParmeter(){Name="NumMatcher",Description="要查找的模型的实例数（0表示所有匹配项）"},
                    new MethodParmeter(){Name="MaxOverlap",Description="要查找的模型实例的最大重叠"},
                    new MethodParmeter(){Name="SubPixel",Description="子像素精度如果不为无"},
                    new MethodParmeter(){Name="NumLevels",Description="匹配中使用的金字塔等级数"},
                    new MethodParmeter(){Name="Greediness",Description="搜索启发式的“贪婪”（0：安全但慢; 1：快，但可能会错过匹配）"},
                    new MethodParmeter(){Name="Row",Description="找到的模型实例的行坐标"},
                    new MethodParmeter(){Name="Column",Description="找到的模型实例的列坐标"},
                    new MethodParmeter(){Name="Angle",Description="找到的模型实例的旋转角度"},
                    new MethodParmeter(){Name="Score",Description="找到的模型实例的得分"},
                },
                Predecessors = new List<string>()
                {
                    "create_shape_model",
                    "read_shape_model",
                    "write_shape_model"
                }
            };
            RunParameter = new ShapeModelRunParameter();
            TemplateParameter = new ShapeModelInputParameter();

            //设置默认参数
            RunParameter.ApplyDefaultParameter();
            TemplateParameter.ApplyDefaultParameter();
        }


        public MethodInfo info { get; set; }
        private ShapeModelInputParameter templateParameter;
        private ShapeModelRunParameter runParameter;
        private HTuple modelId = new HTuple();
        HTuple hv_Row = new HTuple(), hv_Col = new HTuple();
        HTuple hv_Angle = new HTuple(), hv_Score = new HTuple();


        public ShapeModelInputParameter TemplateParameter
        {
            get { return templateParameter; }
            set { templateParameter = value; RaisePropertyChanged(); }
        }


        public ShapeModelRunParameter RunParameter
        {
            get { return runParameter; }
            set { runParameter = value; RaisePropertyChanged(); }
        }
        public async Task CreateTemplate(HObject image, HObject hObject)
        {
            await Task.Run(() =>
            {
                HObject template;
                HOperatorSet.GenEmptyObj(out template);
                HOperatorSet.ReduceDomain(image, hObject, out template);

                HOperatorSet.CreateShapeModel(template,
                    TemplateParameter.NumLevels,
                    TemplateParameter.AngleStart,
                    TemplateParameter.AngleExtent,
                    TemplateParameter.AngleStep,
                    TemplateParameter.Optimization,
                    TemplateParameter.Metric,
                    TemplateParameter.Contrast,
                    TemplateParameter.MinContrast, out modelId);
            });
        }

        public MatchResult Run(HObject image)
        {
            MatchResult matchResult = new MatchResult();
            matchResult.Results = new ObservableCollection<TemplateMatchResult>();

            var timeSpan = SetTimer(() =>
            {
                HOperatorSet.FindShapeModel(
                            image,
                            modelId,
                            RunParameter.AngleStart,
                            RunParameter.AngleExtent,
                            RunParameter.MinScore,
                            RunParameter.NumMatches,
                            RunParameter.Maxoverlap,
                            RunParameter.SubPixel,
                            RunParameter.NumLevels,
                            RunParameter.Greediness,
                            out hv_Row, out hv_Col, out hv_Angle, out hv_Score);
            });

            for (int i = 0; i < hv_Score.Length; i++)
            {
                matchResult.Results.Add(new TemplateMatchResult()
                {
                    Index = i + 1,
                    Row = hv_Row[i],
                    Column = hv_Col[i],
                    Angle = hv_Angle.DArr[i],
                    Score = hv_Score.DArr[i]
                });
            }
            matchResult.TimeSpan = timeSpan;
            return matchResult;
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
    /// 形状匹配模板参数
    /// </summary>
    public class ShapeModelInputParameter : BaseParameter
    {
        private string numLevels;
        private double angleStart;
        private double angleExtent;
        private string angleStep;
        private string optimization;
        private string metric;
        private string contrast;
        private string minContrast;

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
            AngleStart = -0.39;
            AngleExtent = 0.79;
            AngleStep = "auto";
            Optimization = "auto";
            Metric = "use_polarity";
            Contrast = "auto_contrast";
            MinContrast = "auto";
        }

    }

    /// <summary>
    /// 形状匹配模板运行参数
    /// </summary>
    public class ShapeModelRunParameter : BaseParameter
    {
        private int numLevels;
        private double angleStart;
        private double angleExtent;
        private double minScore;
        private double greediness;
        private string subPixel;
        private double maxOverlap;
        private int numMatches;

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
        /// 精度设置
        /// </summary>
        public string SubPixel
        {
            get { return subPixel; }
            set { subPixel = value; RaisePropertyChanged(); }
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
            AngleStart = -0.39;
            AngleExtent = 0.79;
            MinScore = 0.5;
            NumMatches = 1;
            Maxoverlap = 0.5;
            SubPixel = "least_squares";
            NumLevels = 0;
            Greediness = 0.9;
        }
    }
}
