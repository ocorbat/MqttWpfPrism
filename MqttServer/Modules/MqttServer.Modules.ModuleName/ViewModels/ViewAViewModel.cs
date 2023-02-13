using MqttCommon;
using MQTTnet;
using MQTTnet.Internal;
using MQTTnet.Server;
using MqttServer.Core.Mvvm;
using MqttServer.Services.Interfaces;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MqttServer.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase
    {
        private MqttFactory mqttFactory = new MqttFactory();
        private MQTTnet.Server.MqttServer mqttServer;
        private string status;
        private IEnumerable<MqttClientStatus> connectedClients;
        private ObservableCollection<MqttClientStatus> subscribedClients = new ObservableCollection<MqttClientStatus>();
        private ICollectionView subscribedClientsView;

        public ViewAViewModel(IRegionManager regionManager, IMessageService messageService) :
            base(regionManager)
        {
            StartServerCommand = new DelegateCommand(StartServerCommandExecute, StartServerCommandCanExecute);
            StopServerCommand = new DelegateCommand(StopServerCommandExecute, StopServerCommandCanExecute);
            GetConnectedClientsCommand = new DelegateCommand(GetConnectedClientsCommandExecute, GetConnectedClientsCommandCanExecute);

            SubscribedClientsView = CollectionViewSource.GetDefaultView(subscribedClients);
        }

        public DelegateCommand StartServerCommand { get; set; }
        public DelegateCommand StopServerCommand { get; set; }
        public DelegateCommand GetConnectedClientsCommand { get; set; }

        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public IEnumerable<MqttClientStatus> ConnectedClients
        {
            get { return connectedClients; }
            set { SetProperty(ref connectedClients, value); }
        }

        public ICollectionView SubscribedClientsView
        {
            get { return subscribedClientsView; }
            set { SetProperty(ref subscribedClientsView, value); }
        }

        private bool GetConnectedClientsCommandCanExecute()
        {
            return mqttServer != null && mqttServer.IsStarted;
        }

        private async void GetConnectedClientsCommandExecute()
        {
            if (mqttServer != null)
            {
                ConnectedClients = await mqttServer.GetClientsAsync();
            }
            else
            {
                ConnectedClients = null;
            }
        }



        private bool StopServerCommandCanExecute()
        {
            return mqttServer != null && mqttServer.IsStarted;
        }

        private async void StopServerCommandExecute()
        {
            await mqttServer.StopAsync();

        }

        private bool StartServerCommandCanExecute()
        {
            return mqttServer == null || !mqttServer.IsStarted;
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
                .WithDefaultEndpointPort(Constants.Port5004)
                .Build();

            mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

            mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
            mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
            mqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
            mqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
            mqttServer.InterceptingSubscriptionAsync += MqttServer_InterceptingSubscriptionAsync;
            mqttServer.StartedAsync += MqttServer_StartedAsync;
            mqttServer.StoppedAsync += MqttServer_StoppedAsync;
            mqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;

            await mqttServer.StartAsync();
        }

        private Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            // Here we only change the topic of the received application message.
            // but also changing the payload etc. is required. Changing the QoS after
            // transmitting is not supported and makes no sense at all.
            //args.ApplicationMessage.Topic += "/manipulated";

            return CompletedTask.Instance;
        }

        private Task MqttServer_StoppedAsync(System.EventArgs arg)
        {
            Status = "Server stopped";
            ConnectedClients = null;
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            GetConnectedClientsCommand.RaiseCanExecuteChanged();
            return CompletedTask.Instance;
        }

        private async Task<Task> MqttServer_StartedAsync(System.EventArgs arg)
        {
            Status = "Server started";
            ConnectedClients = await mqttServer.GetClientsAsync();
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            GetConnectedClientsCommand.RaiseCanExecuteChanged();
            return CompletedTask.Instance;
        }

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            var toto = arg.ClientId;

            return Task.CompletedTask;
        }

        private Task MqttServer_InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs arg)
        {
            arg.CloseConnection = false;
            return CompletedTask.Instance;
        }

        private async Task<Task> MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            var id = arg.ClientId;
            var connectedClients = await mqttServer.GetClientsAsync();
            var toto = connectedClients.SingleOrDefault(c => c.Id == id);

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!subscribedClients.Any(c => c.Id == id))
                {
                    subscribedClients.Add(toto);
                }
            });

            return CompletedTask.Instance;
        }

        private Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
        {
            var id = arg.ClientId;
            var toto = subscribedClients.SingleOrDefault(c => c.Id == id);

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                subscribedClients.Remove(toto);
            });

            return CompletedTask.Instance;
        }

        //private async Task MqttServer_InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs arg)
        //{


        //    //if (context.TopicFilter.Topic.StartsWith("admin/foo/bar") && context.ClientId != "theAdmin")
        //    //{
        //    //    context.AcceptSubscription = false;


        //    //}

        //    //if (context.TopicFilter.Topic.StartsWith("the/secret/stuff") && context.ClientId != "Imperator")
        //    //{
        //    //    context.AcceptSubscription = false;
        //    //    context.CloseConnection = true;
        //    //}

        //}

        private async Task<Task> MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            ConnectedClients = await mqttServer.GetClientsAsync();

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return CompletedTask.Instance;
        }

        private async Task<Task> MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            ConnectedClients = await mqttServer.GetClientsAsync();

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return CompletedTask.Instance;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
