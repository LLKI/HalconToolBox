using HalconDotNet;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace MachineVision.Core.TemplateMatch.NccModel
{
    public class NccModelService : BindableBase, ITemplateMatchService
    {
        public NccModelService()
        {
            info = new MethodInfo()
            {
                Name = "find_ncc_model",
                Description = "Find the best matches of an NCC model in an image.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter() { Name = "Image", Description = "输入图像" },
                    new MethodParmeter() { Name = "ModelID", Description = "模型的句柄" },
                    new MethodParmeter() { Name = "AngleStart", Description = "模型的最小旋转" },
                    new MethodParmeter() { Name = "AngleExtent", Description = "旋转角度的范围" },
                    new MethodParmeter() { Name = "MinScore", Description = "要查找的模型实例的最小得分" },
                    new MethodParmeter() { Name = "NumMatcher", Description = "要查找的模型的实例数（0表示所有匹配项）" },
                    new MethodParmeter() { Name = "MaxOverlap", Description = "要查找的模型实例的最大重叠" },
                    new MethodParmeter() { Name = "SubPixel", Description = "子像素精度如果不为无" },
                    new MethodParmeter() { Name = "NumLevels", Description = "匹配中使用的金字塔等级数" },
                    new MethodParmeter() { Name = "Row", Description = "找到的模型实例的行坐标" },
                    new MethodParmeter() { Name = "Column", Description = "找到的模型实例的列坐标" },
                    new MethodParmeter() { Name = "Angle", Description = "找到的模型实例的旋转角度" },
                    new MethodParmeter() { Name = "Score", Description = "找到的模型实例的得分" },
                },
                Predecessors = new List<string>()
                {
                    "create_ncc_model",
                    "read_ncc_model",
                    "write_ncc_model"
                }
            };
            RunParameter = new NccModelRunParameter();
            TemplateParameter = new NccModelInputParameter();
            Setting = new MatchResultSetting();

            //设置默认参数
            RunParameter.ApplyDefaultParameter();
            TemplateParameter.ApplyDefaultParameter();
        }
        public HObject RoiObject { get; set; }
        public MethodInfo info { get; set; }
        public MatchResultSetting Setting { get; set; }
        private NccModelInputParameter templateParameter;
        private NccModelRunParameter runParameter;
        private HTuple modelId = new HTuple();
        HTuple hv_Row = new HTuple(), hv_Col = new HTuple();
        HTuple hv_Angle = new HTuple(), hv_Score = new HTuple();

        public NccModelInputParameter TemplateParameter
        {
            get { return templateParameter; }
            set { templateParameter = value; RaisePropertyChanged(); }
        }

        public NccModelRunParameter RunParameter
        {
            get { return runParameter; }
            set { runParameter = value; RaisePropertyChanged(); }
        }

        public async Task CreateTemplate(HObject image, HObject hObject)
        {
            HOperatorSet.Rgb1ToGray(image, out HObject grayImage);
            await Task.Run(() =>
            {
                HObject template;
                HOperatorSet.GenEmptyObj(out template);
                HOperatorSet.ReduceDomain(grayImage, hObject, out template);

                HOperatorSet.CreateNccModel(template,
                    TemplateParameter.NumLevels,
                    TemplateParameter.AngleStart,
                    TemplateParameter.AngleExtent,
                    TemplateParameter.AngleStep,
                    TemplateParameter.Metric, out modelId);
            });
        }

        public MatchResult Run(HObject image)
        {
            MatchResult matchResult = new MatchResult();
            matchResult.Results = new ObservableCollection<TemplateMatchResult>();

            if (image == null)
            {
                matchResult.Message = "输入图像无效!";
                return matchResult;
            }
            if (modelId == null)
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
                }
                else
                {
                    imageReduced = image;
                }
                HOperatorSet.Rgb1ToGray(imageReduced,out HObject imageReducedGray);
                HOperatorSet.FindNccModel(
                            imageReducedGray,
                            modelId,
                            RunParameter.AngleStart,
                            RunParameter.AngleExtent,
                            RunParameter.MinScore,
                            RunParameter.NumMatches,
                            RunParameter.Maxoverlap,
                            RunParameter.SubPixel,
                            RunParameter.NumLevels,
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
                    Score = hv_Score.DArr[i],
                    Contours = GetNccModelContours(modelId, hv_Row.DArr[i], hv_Col.DArr[i], hv_Angle.DArr[i],0)
                }) ;
            }
            matchResult.TimeSpan = timeSpan;
            matchResult.Setting = Setting;
            matchResult.Message = $"匹配耗时:{timeSpan} ms , 匹配个数:{matchResult.Results.Count}";
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

        /// <summary>
        /// 获取相关性匹配的结果轮廓
        /// </summary>
        /// <param name="hv_ModelID"></param>
        /// <param name="hv_Row"></param>
        /// <param name="hv_Column"></param>
        /// <param name="hv_Angle"></param>
        /// <param name="hv_Model"></param>
        /// <returns></returns>
        public HObject GetNccModelContours(HTuple hv_ModelID,HTuple hv_Row, HTuple hv_Column, HTuple hv_Angle, HTuple hv_Model)
        {
            HObject ho_ModelRegion = null, ho_ModelContours = null;
            HObject ho_ContoursAffinTrans = null, ho_Cross = null;

            // Local control variables 

            HTuple hv_NumMatches = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Match = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_HomMat2DRotate = new HTuple(), hv_HomMat2DTranslate = new HTuple();
            HTuple hv_RowTrans = new HTuple(), hv_ColTrans = new HTuple();
            HTuple hv_Model_COPY_INP_TMP = new HTuple(hv_Model);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelRegion);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            //This procedure displays the results of Correlation-Based Matching.
            //
            hv_NumMatches.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_NumMatches = new HTuple(hv_Row.TupleLength()
                    );
            }
            if ((int)(new HTuple(hv_NumMatches.TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    hv_Model_COPY_INP_TMP.Dispose();
                    HOperatorSet.TupleGenConst(hv_NumMatches, 0, out hv_Model_COPY_INP_TMP);
                }
                else if ((int)(new HTuple((new HTuple(hv_Model_COPY_INP_TMP.TupleLength()
                    )).TupleEqual(1))) != 0)
                {
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleGenConst(hv_NumMatches, hv_Model_COPY_INP_TMP, out ExpTmpOutVar_0);
                        hv_Model_COPY_INP_TMP.Dispose();
                        hv_Model_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ModelID.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ModelRegion.Dispose();
                        HOperatorSet.GetNccModelRegion(out ho_ModelRegion, hv_ModelID.TupleSelect(
                            hv_Index));
                    }
                    ho_ModelContours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_ModelRegion, out ho_ModelContours, "border_holes");
                    HTuple end_val13 = hv_NumMatches - 1;
                    HTuple step_val13 = 1;
                    for (hv_Match = 0; hv_Match.Continue(end_val13, step_val13); hv_Match = hv_Match.TupleAdd(step_val13))
                    {
                        if ((int)(new HTuple(hv_Index.TupleEqual(hv_Model_COPY_INP_TMP.TupleSelect(
                            hv_Match)))) != 0)
                        {
                            hv_HomMat2DIdentity.Dispose();
                            HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HomMat2DRotate.Dispose();
                                HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, hv_Angle.TupleSelect(
                                    hv_Match), 0, 0, out hv_HomMat2DRotate);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HomMat2DTranslate.Dispose();
                                HOperatorSet.HomMat2dTranslate(hv_HomMat2DRotate, hv_Row.TupleSelect(
                                    hv_Match), hv_Column.TupleSelect(hv_Match), out hv_HomMat2DTranslate);
                            }
                            ho_ContoursAffinTrans.Dispose();
                            HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ContoursAffinTrans,hv_HomMat2DTranslate);
                        }
                    }
                }
            }
            ho_ModelRegion.Dispose();
            ho_ModelContours.Dispose();

            hv_Model_COPY_INP_TMP.Dispose();
            hv_NumMatches.Dispose();
            hv_Index.Dispose();
            hv_Match.Dispose();
            hv_HomMat2DIdentity.Dispose();
            hv_HomMat2DRotate.Dispose();
            hv_HomMat2DTranslate.Dispose();
            hv_RowTrans.Dispose();
            hv_ColTrans.Dispose();

            return ho_ContoursAffinTrans;
        }
    }

    /// <summary>
    /// 相似性匹配模板参数
    /// </summary>
    public class NccModelInputParameter : BaseParameter
    {
        private string numLevels;
        private double angleStart;
        private double angleExtent;
        private string angleStep;
        private string metric;

        /// <summary>
        /// 匹配方法
        /// </summary>
        public string Metric
        {
            get { return metric; }
            set { metric = value; RaisePropertyChanged(); }
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
            Metric = "use_polarity";
        }

    }

    /// <summary>
    /// 相似性匹配模板运行参数
    /// </summary>
    public class NccModelRunParameter : BaseParameter
    {
        private double angleStart;
        private double angleExtent;
        private double minScore;
        private int numMatches;
        private double maxOverlap;
        private string subPixel;
        private int numLevels;

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
            AngleStart = 0;
            AngleExtent = 360;
            MinScore = 0.5;
            NumMatches = 10;
            Maxoverlap = 0.5;
            SubPixel = "true";
            NumLevels = 0;
        }
    }
}
