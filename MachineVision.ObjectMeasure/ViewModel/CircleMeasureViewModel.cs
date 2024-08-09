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
    public class CircleMeasureViewModel:NavigationViewModel
    {
        public CircleMeasureViewModel(CircleMeasureService circleMeasureService)
        {
            CircleMeasureService = circleMeasureService;
            RunCommand = new DelegateCommand(Run);
            LoadImageCommand = new DelegateCommand(LoadImage);
            GetParameterCommand = new DelegateCommand(GetParameter);
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
        }

        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand SetRangeCommand { get; set; }
        public DelegateCommand LoadImageCommand { get; set; }
        public DelegateCommand GetParameterCommand { get; set; }
        public CircleMeasureService CircleMeasureService { get; }

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
            set { message = value;RaisePropertyChanged(); }
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

        private void GetParameter()
        {
            var obj = DrawObjectList.FirstOrDefault(x => x.ShapeType == ShapeType.Circle);
            if (obj != null)
            {
                CircleMeasureService.RunParameter.Row = obj.HTuples[0];
                CircleMeasureService.RunParameter.Column = obj.HTuples[1];
                CircleMeasureService.RunParameter.Radius = obj.HTuples[2];
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        private void Run()
        {
            Message = CircleMeasureService.Run(Image);
        }
    }
}
 