using MqttCommon;
using MQTTnet.Internal;
using MQTTnet.Server;
using MqttServer.Core.ViewModels;
using MqttServer.Services.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MqttServer.Modules.ModuleExecute.ViewModels
{
    public class ExecuteViewModel : BindableBase, IServerViewModelBase
    {
        private string status = string.Empty;
        private ObservableCollection<MqttClientStatus> subscribedClients;

        public ExecuteViewModel()
        {
            StartServerCommand = new DelegateCommand(StartServerCommandExecute, StartServerCommandCanExecute);
            StopServerCommand = new DelegateCommand(StopServerCommandExecute, StopServerCommandCanExecute);

            subscribedClients = new ObservableCollection<MqttClientStatus>();
        }


        public DelegateCommand StartServerCommand { get; set; }
        public DelegateCommand StopServerCommand { get; set; }

        private bool StartServerCommandCanExecute()
        {
            return MqttServerController == null ? false : MqttServerController.MqttServer == null || !MqttServerController.MqttServer.IsStarted;
        }

        private async void StartServerCommandExecute()
        {
            // Start server
            // The port for the default endpoint is 1883.
            // The default endpoint is NOT encrypted!
            // Use the builder classes where possible.
            var mqttServerOptions = MqttServerController.MqttFactory.CreateServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(Constants.Port5004)
                .Build();



            MqttServerController.MqttServer = MqttServerController.MqttFactory.CreateMqttServer(mqttServerOptions);

            MqttServerController.MqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
            MqttServerController.MqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            MqttServerController.MqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
            MqttServerController.MqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
            MqttServerController.MqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
            MqttServerController.MqttServer.InterceptingSubscriptionAsync += MqttServer_InterceptingSubscriptionAsync;
            MqttServerController.MqttServer.StartedAsync += MqttServer_StartedAsync;
            MqttServerController.MqttServer.StoppedAsync += MqttServer_StoppedAsync;
            MqttServerController.MqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;

            await MqttServerController.MqttServer.StartAsync();
        }

        private bool StopServerCommandCanExecute()
        {
            return MqttServerController == null ? false : MqttServerController.MqttServer != null && MqttServerController.MqttServer.IsStarted;
        }

        private async void StopServerCommandExecute()
        {

            await MqttServerController.MqttServer.StopAsync();
        }


        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private IEnumerable<MqttClientStatus> connectedClients;
        public IEnumerable<MqttClientStatus> ConnectedClients
        {
            get => connectedClients;
            set => SetProperty(ref connectedClients, value);
        }


        private async Task<Task> MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            ConnectedClients = await MqttServerController.MqttServer.GetClientsAsync();

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return CompletedTask.Instance;
        }

        private async Task<Task> MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            ConnectedClients = await MqttServerController.MqttServer.GetClientsAsync();

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return CompletedTask.Instance;
        }

        private async Task<Task> MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            var id = arg.ClientId;
            var connectedClients = await MqttServerController.MqttServer.GetClientsAsync();
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

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            if (arg.UserName != "admin" || arg.Password != "1234")
            {
                arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                return CompletedTask.Instance;
            }

            arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
            return Task.CompletedTask;
        }

        private Task MqttServer_InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs arg)
        {
            arg.CloseConnection = false;
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

        private async Task<Task> MqttServer_StartedAsync(System.EventArgs arg)
        {
            Status = "Server started";
            ConnectedClients = await MqttServerController.MqttServer.GetClientsAsync();
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            MqttServerController.OnServerStarted(arg);
            //GetConnectedClientsCommand.RaiseCanExecuteChanged();
            return CompletedTask.Instance;
        }

        private Task MqttServer_StoppedAsync(System.EventArgs arg)
        {
            Status = "Server stopped";
            ConnectedClients = null;
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            MqttServerController.OnServerStopped(arg);
            //GetConnectedClientsCommand.RaiseCanExecuteChanged();
            return CompletedTask.Instance;
        }

        private Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            // Here we only change the topic of the received application message.
            // but also changing the payload etc. is required. Changing the QoS after
            // transmitting is not supported and makes no sense at all.
            //args.ApplicationMessage.Topic += "/manipulated";

            return CompletedTask.Instance;
        }














        private IMqttServerController mqttServerController;

        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    StartServerCommand.RaiseCanExecuteChanged();
                    StopServerCommand.RaiseCanExecuteChanged();
                }
            }
        }
    }
}
