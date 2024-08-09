using HalconToolBox.Models;
using Prism.Mvvm;
using System.Collections.ObjectModel;


namespace HalconToolBox.Services
{
    internal class NavigationMenuService :BindableBase, INavigationMenuService
    {

        public NavigationMenuService()
        {
            Items = new ObservableCollection<NavigationItem>();
        }
        private ObservableCollection<NavigationItem> items;

        public ObservableCollection<NavigationItem> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged(); }
        }

        public void InitMenus()
        {
            Items.Clear();

            Items.Add(new NavigationItem("", "全部", "", new ObservableCollection<NavigationItem>()
            {
                new NavigationItem("","模板匹配","",new ObservableCollection<NavigationItem>()
                {
                    new NavigationItem("ShapeOutline","形状匹配","ShapeView"),
                    new NavigationItem("Clouds","相似性匹配","NccView"),
                    new NavigationItem("ShapeOvalPlus","形变匹配",""),
                }),
                new NavigationItem("","比较测量","",new ObservableCollection<NavigationItem>()
                {
                    new NavigationItem("Circle","卡尺找圆","CircleMeasureView"),
                    new NavigationItem("Palette","颜色检测",""),
                    new NavigationItem("Ruler","几何测量",""),
                }),
                new NavigationItem("","字符识别","",new ObservableCollection<NavigationItem>()
                {
                    new NavigationItem("FormatColorText","字符识别",""),
                    new NavigationItem("Barcode","一维码识别","BarCodeView"),
                    new NavigationItem("Qrcode","二维码识别","QRCodeView"),
                }),
                new NavigationItem("","缺陷检测","",new ObservableCollection<NavigationItem>()
                {
                    new NavigationItem("Crop","差分模型",""),
                    new NavigationItem("CropRotate","形变模型","")
                })
            }));
            //Items.Add(new NavigationItem("", "模板匹配", ""));
            //Items.Add(new NavigationItem("", "比较测量", ""));
            //Items.Add(new NavigationItem("", "字符识别", ""));
            //Items.Add(new NavigationItem("", "缺陷检测", ""));
            Items.Add(new NavigationItem("", "学习文档", ""));
        }
    }
}
