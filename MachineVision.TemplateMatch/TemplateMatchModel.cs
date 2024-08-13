using MachineVision.Core.ObjectMeasure;
using MachineVision.TemplateMatch.ViewModels;
using MachineVision.TemplateMatch.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace MachineVision.TemplateMatch
{
    public class TemplateMatchModel : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DrawShapeView, DrawShapeViewModel>();
            containerRegistry.RegisterForNavigation<ShapeView,ShapeViewModel>();
            containerRegistry.RegisterForNavigation<NccView,NccViewModel>();
            containerRegistry.RegisterForNavigation<QRCodeView,QRCodeViewModel>();
            containerRegistry.RegisterForNavigation<BarCodeView, BarCodeViewModel>();
            containerRegistry.RegisterForNavigation<CharRecognitionView, CharRecognitionViewModel>();
            containerRegistry.RegisterForNavigation<DeformableShapeView, DeformableShapeViewModel>();
        }
    }
}
