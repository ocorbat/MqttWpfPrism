using MqttClient.Core;
using MqttClient.Modules.ModuleSubscriber.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttClient.Modules.ModuleSubscriber
{
    public class ModuleSubscriberModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleSubscriberModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.SubscriberRegion, "SubscriberView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SubscriberView>();
        }
    }
}