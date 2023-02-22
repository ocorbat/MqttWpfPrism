
using MqttClient.Backend.Core;
using MqttClient.Modules.ModuleConnect;
using MqttClient.Modules.ModuleMessage;
using MqttClient.Modules.ModuleMessageHistory;
using MqttClient.Modules.ModulePublisher;
using MqttClient.Modules.ModuleSubscriber;
using MqttClient.ViewModels;
using MqttClient.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MqttClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MainWindow mainWindow;
        private IMainWindowDataContext dataContext;
        private StartupEventArgs startupEventArgs;
        protected override Window CreateShell()
        {
            mainWindow = Container.Resolve<MainWindow>();
            dataContext = mainWindow.DataContext as IMainWindowDataContext;

            string applicationTitle = "MQTT Client";
            if (startupEventArgs.Args.Count() == 0 || (startupEventArgs.Args.Contains("publisher") && startupEventArgs.Args.Contains("subscriber")))
            {
                dataContext.Title = applicationTitle;
            }
            else if (startupEventArgs.Args.Contains("publisher"))
            {
                dataContext.Title = applicationTitle + " - Publisher";
            }
            else if (startupEventArgs.Args.Contains("subscriber"))
            {
                dataContext.Title = applicationTitle + " - Subscriber";
            }

            return mainWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            containerRegistry.Register<IMqttClientController, MqttClientController>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleConnectModule>();
            moduleCatalog.AddModule<ModuleMessageHistoryModule>();
            moduleCatalog.AddModule<ModuleMessageModule>();

            if (startupEventArgs.Args.Count() == 0)
            {
                moduleCatalog.AddModule<ModulePublisherModule>();
                moduleCatalog.AddModule<ModuleSubscriberModule>();
            }
            else
            {
                if (startupEventArgs.Args.Contains("publisher"))
                {
                    moduleCatalog.AddModule<ModulePublisherModule>();
                }
                if (startupEventArgs.Args.Contains("subscriber"))
                {
                    moduleCatalog.AddModule<ModuleSubscriberModule>();
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            startupEventArgs = e;
            base.OnStartup(e);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Process currentProcess = Process.GetCurrentProcess();
            var runningMqttClientProcesses = Process.GetProcesses().AsEnumerable<Process>().Where(x => x.ProcessName.Equals("MqttClient")).ToArray();
            var numberOfInstance = runningMqttClientProcesses.Except(runningMqttClientProcesses.Where(x => x.Id == currentProcess.Id)).Count() + 1;

            dataContext.MqttClientController.NumberOfInstance = numberOfInstance;
            dataContext.Title += $" ({dataContext.MqttClientController.NumberOfInstance})";
        }
    }
}
