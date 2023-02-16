using MqttServer.Core;
using MqttServer.Modules.ModuleName.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttServer.Modules.ModuleName
{
    public class ModuleClientsModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleClientsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "ClientsView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ClientsView>();
        }
    }
}