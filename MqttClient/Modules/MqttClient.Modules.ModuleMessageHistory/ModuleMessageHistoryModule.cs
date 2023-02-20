using MqttClient.Core;
using MqttClient.Modules.ModuleMessageHistory.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttClient.Modules.ModuleMessageHistory
{
    public class ModuleMessageHistoryModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleMessageHistoryModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MessageHistoryRegion, nameof(MessageHistoryView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MessageHistoryView>();
        }
    }
}