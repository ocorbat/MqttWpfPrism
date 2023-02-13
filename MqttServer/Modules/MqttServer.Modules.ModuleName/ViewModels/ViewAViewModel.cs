using MQTTnet;
using MQTTnet.Internal;
using MQTTnet.Server;
using MqttServer.Core.Mvvm;
using MqttServer.Services.Interfaces;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MqttServer.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase
    {
        private MqttFactory mqttFactory = new MqttFactory();
        private MQTTnet.Server.MqttServer mqttServer;

        public ViewAViewModel(IRegionManager regionManager, IMessageService messageService) :
            base(regionManager)
        {
            StartServerCommand = new DelegateCommand(StartServerCommandExecute, StartServerCommandCanExecute);
            StopServerCommand = new DelegateCommand(StopServerCommandExecute, StopServerCommandCanExecute);
        }

        private bool StopServerCommandCanExecute()
        {
            return mqttServer == null ? false : mqttServer.IsStarted;
        }

        private async void StopServerCommandExecute()
        {
            await mqttServer.StopAsync();
            Status = "Server stopped";
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
        }

        private bool StartServerCommandCanExecute()
        {
            return mqttServer == null ? true : !mqttServer.IsStarted;
        }

        private async void StartServerCommandExecute()
        {
            // Start server
            // The port for the default endpoint is 1883.
            // The default endpoint is NOT encrypted!
            // Use the builder classes where possible.
            //var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build()
            var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder()
                .WithDefaultEndpoint()
            .Build();
            mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

            mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
            mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
            mqttServer.ValidatingConnectionAsync += e =>
            {
                var toto = e.ClientId;

                return Task.CompletedTask;
            };

            // Processing the incoming application message
            mqttServer.InterceptingPublishAsync += args =>
            {
                // Here we only change the topic of the received application message.
                // but also changing the payload etc. is required. Changing the QoS after
                // transmitting is not supported and makes no sense at all.
                //args.ApplicationMessage.Topic += "/manipulated";

                return CompletedTask.Instance;
            };

            await mqttServer.StartAsync();

            Status = "Server started";

            var toto = await mqttServer.GetClientsAsync();
            ConnectedClients = new ObservableCollection<string>(toto.Select(item => item.Id));



            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
        }

        private static Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            // will never be called, problem when client subscribes to topic
            return new Task(() => { });
        }

        private async Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            var toto = await mqttServer.GetClientsAsync();
            ConnectedClients = new ObservableCollection<string>(toto.Select(item => item.Id));

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return;
        }

        private async Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            var toto = await mqttServer.GetClientsAsync();
            ConnectedClients = new ObservableCollection<string>(toto.Select(item => item.Id));

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }

        public DelegateCommand StartServerCommand { get; set; }

        public DelegateCommand StopServerCommand { get; set; }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        //private ObservableCollection<MqttClientStatus> connectedClients;
        //public ObservableCollection<MqttClientStatus> ConnectedClients
        //{
        //    get { return connectedClients; }
        //    set { SetProperty(ref connectedClients, value); }
        //}

        private ObservableCollection<string> connectedClients;
        public ObservableCollection<string> ConnectedClients
        {
            get { return connectedClients; }
            set { SetProperty(ref connectedClients, value); }
        }
    }
}
