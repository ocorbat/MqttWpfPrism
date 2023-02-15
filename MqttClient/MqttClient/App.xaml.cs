
using MqttClient.Modules.ModuleConnect;
using MqttClient.Modules.ModuleMessage;
using MqttClient.Modules.ModulePublisher;
using MqttClient.Modules.ModuleSubscriber;
using MqttClient.Services;
using MqttClient.Services.Interfaces;
using MqttClient.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace MqttClient
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
            containerRegistry.Register<IMqttClientController, MqttClientController>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleConnectModule>();
            moduleCatalog.AddModule<ModuleMessageModule>();
            moduleCatalog.AddModule<ModulePublisherModule>();
            moduleCatalog.AddModule<ModuleSubscriberModule>();
        }
    }
}
