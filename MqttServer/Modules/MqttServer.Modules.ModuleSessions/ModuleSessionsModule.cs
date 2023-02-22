using MqttServer.Core;
using MqttServer.Modules.ModuleSessions.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttServer.Modules.ModuleSessions
{
    public class ModuleSessionsModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleSessionsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.SessionsRegion, nameof(SessionsView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SessionsView>();
        }
    }
}