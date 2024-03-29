﻿using MqttServer.Core;
using MqttServer.Modules.ModuleClients.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttServer.Modules.ModuleClients
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
            _regionManager.RequestNavigate(RegionNames.ClientsRegion, "ClientsView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ClientsView>();
        }
    }
}