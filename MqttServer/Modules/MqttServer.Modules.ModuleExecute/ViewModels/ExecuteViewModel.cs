using Prism.Commands;
using Prism.Mvvm;

namespace MqttServer.Modules.ModuleExecute.ViewModels
{
    public class ExecuteViewModel : BindableBase
    {
        private string status;


        public ExecuteViewModel()
        {
            StartServerCommand = new DelegateCommand(StartServerCommandExecute, StartServerCommandCanExecute);
            StopServerCommand = new DelegateCommand(StopServerCommandExecute, StopServerCommandCanExecute);
        }


        public DelegateCommand StartServerCommand { get; set; }
        public DelegateCommand StopServerCommand { get; set; }

        private bool StartServerCommandCanExecute()
        {
            return true;
            //return mqttServer == null || !mqttServer.IsStarted;
        }

        private async void StartServerCommandExecute()
        {
            // Start server
            // The port for the default endpoint is 1883.
            // The default endpoint is NOT encrypted!
            // Use the builder classes where possible.
            //var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder()
            //    .WithDefaultEndpoint()
            //    .WithDefaultEndpointPort(Constants.Port5004)
            //    .Build();



            //mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

            //mqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
            //mqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
            //mqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
            //mqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
            //mqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
            //mqttServer.InterceptingSubscriptionAsync += MqttServer_InterceptingSubscriptionAsync;
            //mqttServer.StartedAsync += MqttServer_StartedAsync;
            //mqttServer.StoppedAsync += MqttServer_StoppedAsync;
            //mqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;

            //await mqttServer.StartAsync();
        }

        private bool StopServerCommandCanExecute()
        {
            return true;
            //return mqttServer != null && mqttServer.IsStarted;
        }

        private async void StopServerCommandExecute()
        {

            //await mqttServer.StopAsync();
        }


        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
    }
}
