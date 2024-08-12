using Prism.Ioc;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch;
using Prism.Commands;
using System.Collections.ObjectModel;
using HalconDotNet;
using Microsoft.Win32;
using MachineVision.Shared.Controls;
using MachineVision.TemplateMatch.Views;
using MachineVision.Core.TemplateMatch.DeformableModel;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class DeformableShapeViewModel : NavigationViewModel
    {
        public ITemplateMatchService MatchService { get; set; }

        public DeformableShapeViewModel(DeformableModelService deformableShapeModelService)
        {
            MatchService = deformableShapeModelService;
            //MatchService = ContainerLocator.Current.Resolve<ITemplateMatchService>(nameof(TemplateMatchType.DeformableShapeModel));
            RunCommand = new DelegateCommand(Run);
            CreateTemplateCommand = new DelegateCommand(CreateTemplate);
            SetRangeCommand = new DelegateCommand(SetRange);
            LoadImageCommand = new DelegateCommand(LoadImage);
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
            MatchResult = new MatchResult();
        }

        private HObject maskObject;

        public HObject MaskObject
        {
            get { return maskObject; }
            set { maskObject = value; RaisePropertyChanged(); }
        }


        private MatchResult matchResult;

        public MatchResult MatchResult
        {
            get { return matchResult; }
            set { matchResult = value; RaisePropertyChanged(); }
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
        public DelegateCommand CreateTemplateCommand { get; set; }
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
                MatchService.RoiObject = new HObject(hobject.HObject);
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
        /// 创建匹配模板
        /// </summary>
        private void CreateTemplate()
        {
            var obj = DrawObjectList.FirstOrDefault();
            if (obj != null)
            {
                if (MaskObject != null)
                {
                    HOperatorSet.Difference(obj.HObject, MaskObject, out HObject regionDiffererce);
                    obj.HObject = regionDiffererce;
                }
                MatchService.CreateTemplate(Image, obj.HObject);
                MatchResult.Message = $"{DateTime.Now}:创建模板成功!";
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        private void Run()
        {
            MatchResult = MatchService.Run(Image);
        }
    }
}
