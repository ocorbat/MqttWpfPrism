using MqttServer.Backend.Core;
using MqttServer.Backend.Services;
using MqttServer.Backend.Services.Interfaces;
using MqttServer.Core;
using MqttServer.Modules.ModuleClients;
using MqttServer.Modules.ModuleExecute;
using MqttServer.Modules.ModuleMessage;
using MqttServer.Modules.ModulePublisher;
using MqttServer.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace MqttServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            containerRegistry.Register<IMqttServerController, MqttServerController>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModulePublisherModule>();
            moduleCatalog.AddModule<ModuleClientsModule>();
            moduleCatalog.AddModule<ModuleExecuteModule>();
            moduleCatalog.AddModule<ModuleMessageModule>();
        }
    }
}
