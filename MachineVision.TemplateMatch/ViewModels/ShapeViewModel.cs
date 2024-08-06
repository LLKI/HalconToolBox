using Prism.Ioc;
using MachineVision.Core;
using MachineVision.Core.TemplateMatch;
using Prism.Commands;
using System.Collections.ObjectModel;
using HalconDotNet;
using Microsoft.Win32;
using MachineVision.Shared.Controls;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class ShapeViewModel: NavigationViewModel
    {
        public ITemplateMatchService MatchService { get; set; }

        public ShapeViewModel() 
        {
            MatchService =  ContainerLocator.Current.Resolve<ITemplateMatchService>(nameof(TemplateMatchType.ShapeModel));
            RunCommand = new DelegateCommand(Run);
            CreateTemplateCommand = new DelegateCommand(CreateTemplate);
            SetRangeCommand = new DelegateCommand(SetRange);
            LoadImageCommand = new DelegateCommand(LoadImage);
            MatchResults = new ObservableCollection<TemplateMatchResult>();
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
        }

        private string txtMeg;

        public string TxtMeg
        {
            get { return txtMeg; }
            set { txtMeg = value; RaisePropertyChanged(); }
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

        private ObservableCollection<TemplateMatchResult> matchResults;

        /// <summary>
        /// 匹配结果集合
        /// </summary>
        public ObservableCollection<TemplateMatchResult> MatchResults
        {
            get { return matchResults; }
            set { matchResults = value; RaisePropertyChanged(); }
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
            if(obj != null)
            {
                MatchService.CreateTemplate(Image,obj.HObject);
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        private void Run()
        {
            var matchResult = MatchService.Run(Image);
            MatchResults = matchResult.Results;
        }

    }
}
