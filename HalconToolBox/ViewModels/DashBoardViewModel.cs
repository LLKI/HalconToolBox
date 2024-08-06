using HalconToolBox.Models;
using HalconToolBox.Services;
using MachineVision.Core;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Security.RightsManagement;

namespace HalconToolBox.ViewModels
{
    public class DashBoardViewModel : NavigationViewModel
    {
        private readonly IRegionManager regionManager;

        public DashBoardViewModel(IRegionManager regionManager, INavigationMenuService navigationService)
        {
            this.regionManager = regionManager;
            NavigationService = navigationService;
            OpenPageCommand = new DelegateCommand<NavigationItem>(OpenPage);
        }

        public INavigationMenuService NavigationService { get; }

        public DelegateCommand<NavigationItem> OpenPageCommand { get;set; }

        private void OpenPage(NavigationItem item)
        {
            regionManager.Regions["MainViewRegion"].RequestNavigate(item.PageName);
        }

        public override void OnNavigatedTo(NavigationContext parameter)
        {
            base.OnNavigatedTo(parameter);
        }
    }
}
