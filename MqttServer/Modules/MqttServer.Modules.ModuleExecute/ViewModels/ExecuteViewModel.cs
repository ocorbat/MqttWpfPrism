using MqttCommon;
using MQTTnet.Internal;
using MQTTnet.Server;
using MqttServer.Core.Dispose;
using MqttServer.Core.Interfaces;
using Prism.Commands;
using System.Threading.Tasks;
using System.Windows;

namespace MqttServer.Modules.ModuleExecute.ViewModels
{
    public class ExecuteViewModel : DisposableBindableBase, IMqttServerControllerViewModel
    {
        // To detect redundant calls
        private bool _disposedValue;

        ~ExecuteViewModel() => Dispose(false);

        private IMqttServerController mqttServerController;
        private string status = string.Empty;

        public ExecuteViewModel()
        {
            StartServerCommand = new DelegateCommand(StartServerCommandExecute, StartServerCommandCanExecute);
            StopServerCommand = new DelegateCommand(StopServerCommandExecute, StopServerCommandCanExecute);
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

            MqttServerController.MqttServer.StartedAsync += MqttServer_StartedAsync;
            MqttServerController.MqttServer.StoppedAsync += MqttServer_StoppedAsync;
            MqttServerController.MqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
            MqttServerController.MqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            MqttServerController.MqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
            MqttServerController.MqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;

            MqttServerController.MqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
            MqttServerController.MqttServer.InterceptingSubscriptionAsync += MqttServer_InterceptingSubscriptionAsync;
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

        private async Task<Task> MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            MqttServerController.OnClientConnected(arg);

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return CompletedTask.Instance;
        }

        private async Task<Task> MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            MqttServerController.OnClientDisconnected(arg);

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
            return CompletedTask.Instance;
        }

        private Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            MqttServerController.OnClientSubscribedTopic(arg);
            return CompletedTask.Instance;
        }

        private Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
        {
            MqttServerController.OnClientUnsubscribedTopic(arg);
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

        private Task MqttServer_StartedAsync(System.EventArgs arg)
        {
            Status = "Server started";
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            MqttServerController.OnServerStarted(arg);
            return CompletedTask.Instance;
        }

        private Task MqttServer_StoppedAsync(System.EventArgs arg)
        {
            Status = "Server stopped";
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
            MqttServerController.OnServerStopped(arg);
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

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                MqttServerController.MqttServer.StartedAsync -= MqttServer_StartedAsync;
                MqttServerController.MqttServer.StoppedAsync -= MqttServer_StoppedAsync;
                MqttServerController.MqttServer.ClientConnectedAsync -= MqttServer_ClientConnectedAsync;
                MqttServerController.MqttServer.ClientDisconnectedAsync -= MqttServer_ClientDisconnectedAsync;
                MqttServerController.MqttServer.ClientSubscribedTopicAsync -= MqttServer_ClientSubscribedTopicAsync;
                MqttServerController.MqttServer.ClientUnsubscribedTopicAsync -= MqttServer_ClientUnsubscribedTopicAsync;
                MqttServerController.MqttServer.ValidatingConnectionAsync -= MqttServer_ValidatingConnectionAsync;
                MqttServerController.MqttServer.InterceptingSubscriptionAsync -= MqttServer_InterceptingSubscriptionAsync;
                MqttServerController.MqttServer.InterceptingPublishAsync -= MqttServer_InterceptingPublishAsync;
                _disposedValue = true;
            }

            // Call the base class implementation.
            base.Dispose(disposing);
        }

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
