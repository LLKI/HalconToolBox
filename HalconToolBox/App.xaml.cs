﻿using System.Configuration;
using System.Data;
using System.Windows;
using HalconToolBox.Services;
using HalconToolBox.ViewModels;
using HalconToolBox.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using MachineVision.TemplateMatch;
using MachineVision.Core.TemplateMatch.ShapeModel;
using MachineVision.Core.TemplateMatch.NccModel;
using MachineVision.Core.TemplateMatch;
using MachineVision.Core.TemplateMatch.OCR;
using MachineVision.ObjectMeasure;
using MachineVision.Core.ObjectMeasure;
using MachineVision.Core.TemplateMatch.DeformableModel;
using MachineVision.DefectDetection;
using HalconDotNet;
using MachineVision.Core.DefectDetection;

namespace HalconToolBox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => null;

        protected override void OnInitialized()
        {
            //从容器中获取MainView的实例对象
            var container = ContainerLocator.Container;
            var shell = container.Resolve<object>("MainView");
            if(shell is Window view)
            {
                //更新Prism注册区域信息
                var regionManager = container.Resolve<IRegionManager>();
                RegionManager.SetRegionManager(view, regionManager);
                RegionManager.UpdateRegions();

                //调用首页的INavigationAware 接口做一个初始化操作
                if(view.DataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(null);
                    //呈现首页
                    App.Current.MainWindow = view;
                }
            }
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry services)
        {
            services.RegisterForNavigation<MainView, MainViewModel>();
            services.RegisterForNavigation<DashBoardView, DashBoardViewModel>();
            services.RegisterForNavigation<StudyFile, StudyFileViewModel>();
            services.RegisterSingleton<INavigationMenuService, NavigationMenuService>();


            services.Register<ITemplateMatchService,ShapeModelService>(nameof(TemplateMatchType.ShapeModel));
            services.Register<ITemplateMatchService,NccModelService>(nameof(TemplateMatchType.NccModel));

            services.Register<BarCodeService>();
            services.Register<QRCodeService>();
            services.Register<CircleMeasureService>();
            services.Register<CharRecognitionService>();
            services.Register<DeformableModelService>();
            services.Register<ColorDetectionService>();
            services.Register<SobelAmpService>();
            services.Register<VariationService>();
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<TemplateMatchModel>();
            moduleCatalog.AddModule<ObjectMeasureModule>();
            moduleCatalog.AddModule<VariationModel>();
            base.ConfigureModuleCatalog(moduleCatalog);
        }
    }
}