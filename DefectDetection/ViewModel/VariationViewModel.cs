using Microsoft.Win32;
using Prism.Ioc;
using MachineVision.Core;
using MachineVision.Core.DefectDetection;
using HalconDotNet;
using MachineVision.Core.TemplateMatch;
using MachineVision.Shared.Controls;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MachineVision.DefectDetection.ViewModel
{
    public class VariationViewModel: NavigationViewModel
    {
        public VariationViewModel(VariationService service)
        {
            Service = service;

            RunCommand = new DelegateCommand(Run);
            LoadTrainImagesCommand = new DelegateCommand(LoadTrainImages);
            SetRangeCommand = new DelegateCommand(SetRange);
            LoadImageCommand = new DelegateCommand(LoadImage);
            TrainCommand = new DelegateCommand(Train);

            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
            TrainImages = new List<string>();
        }

        public VariationService Service { get; }

        public List<string> TrainImages { get; set; }

        private HObject maskObject;

        public HObject MaskObject
        {
            get { return maskObject; }
            set { maskObject = value; RaisePropertyChanged(); }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(); }
        }


        private HObject image;

        public HObject Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<DrawingObjectInfo> drawObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawObjectList
        {
            get { return drawObjectList; }
            set { drawObjectList = value; RaisePropertyChanged(); }
        }

        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand LoadTrainImagesCommand { get; set; }
        public DelegateCommand TrainCommand { get; set; }
        public DelegateCommand SetRangeCommand { get; set; }
        public DelegateCommand LoadImageCommand { get; set; }

        /// <summary>
        /// 设置ROI区域
        /// </summary>
        private void SetRange()
        {
            var hobject = DrawObjectList.FirstOrDefault();
            DrawObjectList.Clear();//清空,给后面创建模板时不拿错
            if (hobject != null)
            {
                Service.RoiObject = new HObject(hobject.HObject);
                Message = $"{DateTime.Now}:设置ROI成功!";
            }
            else
            {
                Message = $"{DateTime.Now}:请绘制ROI识别范围后点击按钮设置!";
            }
        }
        /// <summary>
        /// 加载图像
        /// </summary>
        private void LoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = (bool)dialog.ShowDialog();
            if (result)
            {
                var img = new HImage();
                img.ReadImage(dialog.FileName);
                Image = img;
            }
        }
        /// <summary>
        /// 读取训练图片
        /// </summary>
        /// <returns></returns>
        public void LoadTrainImages()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择图片";
            openFileDialog.Filter = "图片文件(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            openFileDialog.Multiselect = true; // 允许选择多个文件

            var result = (bool)openFileDialog.ShowDialog();
            if (result)
            {
                // 循环遍历所有选定的文件路径并将它们添加到列表中
                foreach (string filePath in openFileDialog.FileNames)
                {
                    TrainImages.Add(filePath);
                }
                Message = "读取训练图片成功!";
            }
            else
            {
                Message = "读取训练图片失败!";
            }
        }

        /// <summary>
        /// 训练模型
        /// </summary>
        private void Train()
        {
            Message = Service.Train(TrainImages);
        }
        /// <summary>
        /// 执行
        /// </summary>
        private void Run()
        {
            Message = Service.Run(Image,TrainImages);
        }
    }
}
