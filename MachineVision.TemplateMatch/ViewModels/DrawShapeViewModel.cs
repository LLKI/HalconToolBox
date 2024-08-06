using HalconDotNet;
using MachineVision.Core;
using MachineVision.Shared.Controls;
using Microsoft.Win32;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MachineVision.TemplateMatch.ViewModels
{
    public class DrawShapeViewModel:NavigationViewModel
    {
        public DrawShapeViewModel()
        {
            LoadImageCommand = new DelegateCommand(LoadImage);
            DrawObjectList = new ObservableCollection<DrawingObjectInfo>();
        }
        private HObject image;

        public HObject Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged();}
        }

        private ObservableCollection<DrawingObjectInfo> drawObjectList;

        public ObservableCollection<DrawingObjectInfo> DrawObjectList
        {
            get { return drawObjectList; }
            set { drawObjectList = value; RaisePropertyChanged(); }
        }


        public DelegateCommand LoadImageCommand { get; set; }

        private void LoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = (bool)dialog.ShowDialog();
            if(result)
            {
                var img = new HImage();
                img.ReadImage(dialog.FileName);
                Image = img;
            }
        }

    }
}
