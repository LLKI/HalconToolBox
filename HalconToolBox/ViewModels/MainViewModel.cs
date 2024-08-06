using HalconToolBox.Models;
using HalconToolBox.Services;
using MachineVision.Core;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalconToolBox.ViewModels
{
    public class MainViewModel:NavigationViewModel
    {
        public MainViewModel(INavigationMenuService navigationService,IRegionManager regionManager)
        {
            NavigationService = navigationService;
            this.regionManager = regionManager;
            NavigateCommand = new DelegateCommand<NavigationItem>(Navigate);
            IsTopDrawerOpen = false;
        }

        public INavigationMenuService NavigationService { get; }
        public DelegateCommand<NavigationItem> NavigateCommand { get; set; }
        private bool isTopDrawerOpen;
        private readonly IRegionManager regionManager;

        public bool IsTopDrawerOpen
        {
            get { return isTopDrawerOpen; }
            set { isTopDrawerOpen = value; RaisePropertyChanged(); }
        }

        private void Navigate(NavigationItem obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj.Name == "全部")
            {
                IsTopDrawerOpen = true;
                return;
            }
            IsTopDrawerOpen = false;
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavigationService.InitMenus();
            NavigatePage("DashBoardView");//默认显示主页
            base.OnNavigatedTo(navigationContext);
        }

        private void NavigatePage(string pageName)
        {
            regionManager.Regions["MainViewRegion"].RequestNavigate(pageName,back=> 
            {
                if(!(bool)back.Result)
                {
                    System.Diagnostics.Debug.WriteLine(back.Error.Message);
                }
            });
        }
    }
}
