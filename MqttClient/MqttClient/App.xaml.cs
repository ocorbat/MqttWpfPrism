using MqttClient.Modules.ModuleConnect;
using MqttClient.Modules.ModuleName;
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
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
            moduleCatalog.AddModule<ModuleConnectModule>();
        }
    }
}
