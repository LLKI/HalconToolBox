using HalconDotNet;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch;
using MachineVision.Core.TemplateMatch.OCR;
using MachineVision.Shared.Controls;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class BarCodeViewModel:NavigationViewModel
    {
        /// <summary>
        /// 一维码识别
        /// </summary>
        /// <param name="barCodeService"></param>
        public BarCodeViewModel(BarCodeService barCodeService)
        {
            BarCodeService = barCodeService;
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
            RunCommand = new DelegateCommand(Run);
            SetRangeCommand = new DelegateCommand(SetRange);
            LoadImageCommand = new DelegateCommand(LoadImage);
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
            MatchResult = new OcrMatchResult();
        }
        public BarCodeService BarCodeService { get; }

        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand SetRangeCommand { get; set; }
        public DelegateCommand LoadImageCommand { get; set; }

        private ObservableCollection<DrawingObjectInfo> drawObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawObjectList
        {
            get { return drawObjectList; }
            set { drawObjectList = value; RaisePropertyChanged(); }
        }

        private OcrMatchResult ocrMatchResult;

        public OcrMatchResult MatchResult
        {
            get { return ocrMatchResult; }
            set { ocrMatchResult = value; RaisePropertyChanged(); }
        }

        private HObject image;

        public HObject Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
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
                BarCodeService.RoiObject = new HObject(hobject.HObject);
                MatchResult.Message = $"{DateTime.Now}:设置ROI成功!";
            }
            else
            {
                MatchResult.Message = $"{DateTime.Now}:请绘制ROI识别范围后点击按钮设置!";
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
        /// 执行
        /// </summary>
        private void Run()
        {
            MatchResult = BarCodeService.Run(image);
        }
    }
}
