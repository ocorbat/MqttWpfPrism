using MqttServer.Core;
using MqttServer.Modules.ModuleExecute.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttServer.Modules.ModuleExecute
{
    public class ModuleExecuteModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleExecuteModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.ExecuteRegion, "ExecuteView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ExecuteView>();
        }
    }
}