using MqttClient.Core;
using MqttClient.Modules.ModulePublisher.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttClient.Modules.ModulePublisher
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