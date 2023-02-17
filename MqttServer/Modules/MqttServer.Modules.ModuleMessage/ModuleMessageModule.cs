﻿using MqttServer.Core;
using MqttServer.Modules.ModuleMessage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MqttServer.Modules.ModuleMessage
{
    public class ModuleMessageModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleMessageModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MessageRegion, "MessageView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MessageView>();
        }
    }
}