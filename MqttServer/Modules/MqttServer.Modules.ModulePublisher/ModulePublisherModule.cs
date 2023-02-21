using MqttServer.Core;
using MqttServer.Modules.ModulePublisher.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttServer.Modules.ModulePublisher
{
    public class ModulePublisherModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModulePublisherModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.PublisherRegion, "PublisherView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<PublisherView>();
        }
    }
}