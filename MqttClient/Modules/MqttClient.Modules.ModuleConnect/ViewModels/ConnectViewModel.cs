using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MqttCommon;
using MqttCommon.Extensions;
using MQTTnet.Client;
using MQTTnet.Formatter;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MqttClient.Modules.ModuleConnect.ViewModels
{
    public class ConnectViewModel : BindableBase, IClientViewModelBase
    {
        private string username = "admin";
        private string password = "1234";
        private string status = string.Empty;
        private bool isCleanSessionOn = true;
        private MqttProtocolVersion protocolVersion = MqttProtocolVersion.V500;

        public ConnectViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
        }

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public bool IsCleanSessionOn
        {
            get => isCleanSessionOn;
            set => SetProperty(ref isCleanSessionOn, value);
        }

        public MqttProtocolVersion ProtocolVersion
        {
            get => protocolVersion;
            set => SetProperty(ref protocolVersion, value);
        }

        private async void ConnectCommandExecute()
        {
            var mqttClientOptions = MqttClientController.MqttFactory.CreateClientOptionsBuilder()
                .WithClientId(MqttClientController.ClientId.ToString())
                .WithTcpServer(Constants.Localhost, Constants.Port5004)
                .WithCleanSession(IsCleanSessionOn)
                .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
                .WithProtocolVersion(ProtocolVersion)
                .WithCredentials(Username, Password)
                .Build();

            MqttClientController.MqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine("Received application message.");
                e.DumpToConsole();

                var payloadString = Convert.ToString(e.ApplicationMessage.Payload);

                // Convert Payload to string
                var payload = e.ApplicationMessage?.Payload == null ? null : System.Text.Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);

                ReceivedMessage = payload;
                Debug.WriteLine(payload);

                return Task.CompletedTask;
            };

            MqttClientController.MqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            MqttClientController.MqttClient.ConnectingAsync += MqttClient_ConnectingAsync;
            MqttClientController.MqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;

            MqttClientConnectResult result;
            try
            {
                result = await MqttClientController.MqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                //Output = result.DumpToString();
            }
            catch (Exception e)
            {
                //ExceptionText = $"({e})";
                Debug.WriteLine($"Timeout while publishing. {e}");
            }
        }

        private bool ConnectCommandCanExecute()
        {
            return MqttClientController != null && (MqttClientController.MqttClient == null || !MqttClientController.MqttClient.IsConnected);
        }

        private async void DisonnectCommandExecute()
        {
            await MqttClientController.MqttClient.DisconnectAsync();
        }

        private bool DisonnectCommandCanExecute()
        {
            return MqttClientController != null && (MqttClientController.MqttClient != null && MqttClientController.MqttClient.IsConnected);
        }


        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Status = $"Client {MqttClientController.ClientId} is connecting";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            MqttClientController?.OnClientConnecting(arg);
            return Task.CompletedTask;
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Status = $"Client {MqttClientController.ClientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            MqttClientController?.OnClientDisconnected(arg);
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Status = $"Client {MqttClientController.ClientId} is connected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            MqttClientController?.OnClientConnected(arg);
            return Task.CompletedTask;
        }







        private string receivedMessage;
        public string ReceivedMessage
        {
            get => receivedMessage;
            set => SetProperty(ref receivedMessage, value);
        }

        //private string output;
        //public string Output
        //{
        //    get => output;
        //    set => SetProperty(ref output, value);
        //}

        //private string exceptionText;
        //public string ExceptionText
        //{
        //    get => exceptionText;
        //    set => SetProperty(ref exceptionText, value);
        //}

        private IMqttClientController mqttClientController;

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    ConnectCommand.RaiseCanExecuteChanged();
                    DisconnectCommand.RaiseCanExecuteChanged();
                }
            }
        }
    }
}
