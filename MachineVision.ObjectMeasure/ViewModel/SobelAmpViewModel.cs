using HalconDotNet;
using MachineVision.Core;
using MachineVision.Core.ObjectMeasure;
using MachineVision.Core.TemplateMatch;
using MachineVision.Shared.Controls;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MachineVision.ObjectMeasure.ViewModel
{
    public class SobelAmpViewModel: NavigationViewModel
    {
        public SobelAmpViewModel(SobelAmpService service)
        {
            Service = service;
            RunCommand = new DelegateCommand(Run);
            LoadImageCommand = new DelegateCommand(LoadImage);
            SetRangeCommand = new DelegateCommand(SetRange);
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
        }

        public SobelAmpService Service { get; }
        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand SetRangeCommand { get; set; }
        public DelegateCommand LoadImageCommand { get; set; }

        private ObservableCollection<DrawingObjectInfo> drawObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawObjectList
        {
            get { return drawObjectList; }
            set { drawObjectList = value; RaisePropertyChanged(); }
        }

        private HObject image;

        public HObject Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(); }
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
        /// 执行
        /// </summary>
        private void Run()
        {
            Message = Service.Run(Image);
        }
    }
}
