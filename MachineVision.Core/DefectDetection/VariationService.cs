using HalconDotNet;
using MachineVision.Core.TemplateMatch.ShapeModel;
using MachineVision.Core.TemplateMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Microsoft.Win32;
using Prism.Services.Dialogs;
using MachineVision.Core.Extensions;

namespace MachineVision.Core.DefectDetection
{
    public class VariationService: BindableBase
    {
        public VariationService()
        {
            info = new MethodInfo()
            {
                Name = "prepare_variation_model",
                Description = "Prepare a variation model for comparison with an image.",
                Parmeters = new List<MethodParmeter>()
                {
                    new MethodParmeter(){Name="ModelID",Description="变异模型的ID"},
                    new MethodParmeter(){Name="AbsThreshold",Description="图像与变异模型之间差异的绝对最小阈值"},
                    new MethodParmeter(){Name="VarThreshold",Description="基于变异模型的变异的差异阈值"},
                },
                Predecessors = new List<string>()
                {
                    "train_variation_model",
                    "compare_variation_model",
                    "compare_ext_variation_model",
                    "get_thresh_images_variation_model",
                    "clear_train_data_variation_model",
                    "write_variation_model",
                }
            };
            RunParameter = new PrepareVariationParameter();
            RunParameter.ApplyDefaultParameter();
        }

        private HWindow hWindow;
        public HWindow HWindow
        {
            get { return hWindow; }
            set { hWindow = value; RaisePropertyChanged(); }
        }

        private PrepareVariationParameter runParameter;
        public PrepareVariationParameter RunParameter
        {
            get { return runParameter; }
            set { runParameter = value; RaisePropertyChanged(); }
        }

        public HObject RoiObject { get; set; }
        public MethodInfo info { get; set; }

        HObject ho_Region, ho_RegionFillUp;
        HObject ho_RegionDifference, ho_RegionTrans, ho_RegionDilation;
        HObject ho_ImageReduced, ho_ModelImages, ho_ModelRegions;
        HObject ho_Model, ho_ImageTrans = null, ho_MeanImage, ho_VarImage;
        HObject ho_RegionDiff = null, ho_ConnectedRegions = null, ho_RegionsError = null;

        HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
        HTuple hv_WindowHandle = new HTuple(), hv_Area = new HTuple();
        HTuple hv_RowRef = new HTuple(), hv_ColumnRef = new HTuple();
        HTuple hv_ShapeModelID = new HTuple(), hv_VariationModelID = new HTuple();
        HTuple hv_I = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
        HTuple hv_Angle = new HTuple(), hv_Score = new HTuple();
        HTuple hv_HomMat2D = new HTuple(), hv_NumImages = new HTuple();
        HTuple hv_NumError = new HTuple();

        public string Train(List<string>TrainImagePaths)
        {
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ModelImages);
            HOperatorSet.GenEmptyObj(out ho_ModelRegions);
            HOperatorSet.GenEmptyObj(out ho_Model);
            HOperatorSet.GenEmptyObj(out ho_ImageTrans);
            HOperatorSet.GenEmptyObj(out ho_MeanImage);
            HOperatorSet.GenEmptyObj(out ho_VarImage);
            HOperatorSet.GenEmptyObj(out ho_RegionDiff);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionsError);

            if(TrainImagePaths.Count ==0)
            {
                return "请先读取训练图片!";
            }
            try
            {
                var timespan = SetTimeHepler.SetTimer(() =>
                {

                    HOperatorSet.ReadImage(out HObject first_image, TrainImagePaths[0]);

                    HObject imageReduced;
                    HOperatorSet.GenEmptyObj(out imageReduced);
                    if (RoiObject != null)
                    {
                        HOperatorSet.ReduceDomain(first_image, (HObject)RoiObject, out imageReduced);
                        first_image = imageReduced;
                    }

                    HOperatorSet.GetImageSize(first_image, out hv_Width, out hv_Height);
                    HOperatorSet.AreaCenter(first_image, out hv_Area, out hv_RowRef, out hv_ColumnRef);
                    HOperatorSet.CreateShapeModel(first_image, 5, (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(), "auto", "none", "use_polarity", 20, 10, out hv_ShapeModelID);
                    HOperatorSet.CreateVariationModel(hv_Width, hv_Height, "byte", "standard", out hv_VariationModelID);

                    for (int i = 1; i < TrainImagePaths.Count(); i++)
                    {
                        string imagePath = TrainImagePaths[i];
                        HOperatorSet.ReadImage(out HObject train_image, imagePath);
                        //先寻找模板的实例
                        HOperatorSet.FindShapeModel(train_image, hv_ShapeModelID, (new HTuple(-10)).TupleRad(), (new HTuple(20)).TupleRad(), 0.5, 1, 0.5, "least_squares", 0, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                        if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleEqual(1))) != 0)
                        {
                            //使用仿射变换，将当前图像平移旋转到与模板图像重合
                            HOperatorSet.VectorAngleToRigid(hv_Row, hv_Column, hv_Angle, hv_RowRef, hv_ColumnRef, 0, out hv_HomMat2D);
                            HOperatorSet.AffineTransImage(train_image, out ho_ImageTrans, hv_HomMat2D, "constant", "false");
                            //训练差异模型
                            HOperatorSet.TrainVariationModel(ho_ImageTrans, hv_VariationModelID);
                            //HWindow.DispObj(ho_ImageTrans);
                            //HWindow.DispObj(ho_Model);
                        }
                    }
                });
                return $"训练成功! 耗时:{DateTime.Now} : {timespan} ms";
            }
            catch (Exception ex)
            {
                return $"训练失败!{DateTime.Now}" + ex.Message;
            }
        }
        public string Run(HObject image,List<string> TrainImagePaths)
        {
            try
            {
                var timespan = SetTimeHepler.SetTimer(() =>
                {
                    //获得差异模型
                    HOperatorSet.GetVariationModel(out ho_MeanImage, out ho_VarImage, hv_VariationModelID);

                    //做检测之前可以先用下面这个算子对可变模型进行设参,这是一个经验值,需要调试者调整
                    HOperatorSet.PrepareVariationModel(hv_VariationModelID, RunParameter.AbsThreshold, RunParameter.VarThreshold);
                    //HOperatorSet.SetDraw(HWindow, "margin");

                    //对图像进行缺陷检测，思想就是差分

                    //要注意做差分的两幅图像分辨率相同，当然也需要通过仿射变换把待检测的图像转到与模板图像重合
                    //先寻找模板的实例
                    HObject imageReduced;
                    HOperatorSet.GenEmptyObj(out imageReduced);
                    if (RoiObject != null)
                    {
                        HOperatorSet.ReduceDomain(image, (HObject)RoiObject, out imageReduced);
                        image = imageReduced;
                    }
                    HOperatorSet.FindShapeModel(image, hv_ShapeModelID, (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(), 0.5, 1, 0.5, "least_squares", 0, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);
                    if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleEqual(1))) != 0)
                    {
                        //使用仿射变换，将当前图像平移旋转到与模板图像重合，注意是当前图像转向模板图像
                        HOperatorSet.VectorAngleToRigid(hv_Row, hv_Column, hv_Angle, hv_RowRef, hv_ColumnRef, 0, out hv_HomMat2D);
                        HOperatorSet.AffineTransImage(image, out ho_ImageTrans, hv_HomMat2D, "constant", "false");
                        HOperatorSet.ReduceDomain(ho_ImageTrans, ho_RegionDilation, out ho_ImageReduced);
                        //差分 （就是检查两幅图像相减，剩下的区域就是不同的地方了，与模板图像不同的地方就是缺陷）
                        //这里可不能用difference做差分啊，halcon为变形模板提供了专门的差分算子：compare_variation_model
                        HOperatorSet.CompareVariationModel(ho_ImageReduced, out ho_RegionDiff, hv_VariationModelID);
                        HOperatorSet.Connection(ho_RegionDiff, out ho_ConnectedRegions);
                        //特征选择：用一些特征来判断这幅图像印刷是否有缺陷，这里使用面积
                        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_RegionsError, "area", "and", 20, 1000000);
                        HOperatorSet.CountObj(ho_RegionsError, out hv_NumError);

                        HOperatorSet.ClearWindow(HWindow);
                        HWindow.DispObj(ho_ImageTrans);

                        HOperatorSet.SetColor(HWindow, "red");
                        HOperatorSet.DispObj(ho_RegionsError, HWindow);
                        HOperatorSet.SetTposition(HWindow, 20, 20);
                        if ((int)(new HTuple(hv_NumError.TupleEqual(0))) != 0)
                        {
                            HOperatorSet.SetColor(HWindow, "green");
                            HOperatorSet.WriteString(HWindow, "OK");
                        }
                        else
                        {
                            HOperatorSet.SetColor(HWindow, "red");
                            HOperatorSet.WriteString(HWindow, "Not OK");
                        }
                    }
                });
                if(hv_Score.Length > 0)
                {
                    return $"{DateTime.Now} 检测成功! 耗时:{timespan} ms";
                }
                else
                {
                    return $"{DateTime.Now} 检测失败,无法定位! 耗时:{timespan} ms";
                }
            }
            catch (Exception ex)
            {
               return $"{DateTime.Now} 检测失败! "+ex.Message;
            }
        }

        //读取图片——外放 ViewModel
        //设置ROI范围——外放 ViewModel

        //创建shapeModel——使用默认参数
        //创建差分模型——使用默认参数
        //读取训练数据集
        //定位、训练——外放

        //创建可变模型——外放
        //对当前图片进行缺陷检测
    }

    public class PrepareVariationParameter: BaseParameter
    {
        private int absThreshold;

        public int AbsThreshold
        {
            get { return absThreshold; }
            set { absThreshold = value; RaisePropertyChanged();}
        }

        private double varThreshold;

        public double VarThreshold
        {
            get { return varThreshold; }
            set { varThreshold = value;RaisePropertyChanged();}
        }

        public override void ApplyDefaultParameter()
        {
            AbsThreshold = 20;
            VarThreshold = 3;
        }
    }
}
