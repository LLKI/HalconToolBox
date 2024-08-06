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
        }
    }
}
