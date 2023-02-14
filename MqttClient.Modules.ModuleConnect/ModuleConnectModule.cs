using MqttClient.Core;
using MqttClient.Modules.ModuleConnect.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttClient.Modules.ModuleConnect
{
    public class ModuleConnectModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleConnectModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.ConnectRegion, "ConnectView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ConnectView>();
        }
    }
}