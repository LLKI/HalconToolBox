using DefectDetection.View;
using MachineVision.DefectDetection.ViewModel;
using Prism.Ioc;
using Prism.Modularity;

namespace MachineVision.DefectDetection
{
    public class VariationModel : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<VariationView,VariationViewModel>();
        }
    }
}
