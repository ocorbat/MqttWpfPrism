using MqttClient.Services.Interfaces;
using MqttCommon;
using MqttCommon.Extensions;
using MQTTnet;
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
    public class ConnectViewModel : BindableBase
    {
        private MqttFactory mqttFactory = new MqttFactory();
        private IMqttClient? mqttClient;
        private Guid clientId;

        private string username = "admin";
        private string password = "1234";

        public ConnectViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
        }

        static ConnectViewModel()
        {

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


        private bool ConnectCommandCanExecute()
        {
            return mqttClient == null ? true : !mqttClient.IsConnected;
        }

        private async void ConnectCommandExecute()
        {
            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = mqttFactory.CreateClientOptionsBuilder()
                .WithClientId(clientId.ToString())
                .WithTcpServer(Constants.Localhost, Constants.Port5004)
                .WithCleanSession(IsCleanSessionOn)
                .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
                .WithProtocolVersion(ProtocolVersion)
                .WithCredentials(Username, Password)
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
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

            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            mqttClient.ConnectingAsync += MqttClient_ConnectingAsync;
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;

            MqttClientConnectResult result;
            try
            {
                result = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                Output = result.DumpToString();
            }
            catch (Exception e)
            {
                ExceptionText = $"({e})";
                Debug.WriteLine($"Timeout while publishing. {e}");
            }
        }

        private string status = string.Empty;
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Status = $"Client {clientId} is connecting";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Status = $"Client {clientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            ReceivedMessage = string.Empty;
            Output = string.Empty;
            ExceptionText = string.Empty;
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Status = $"Client {clientId} is connected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            ReceivedMessage = string.Empty;
            Output = string.Empty;
            ExceptionText = string.Empty;
            return Task.CompletedTask;
        }

        private bool DisonnectCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void DisonnectCommandExecute()
        {
            await mqttClient.DisconnectAsync();
        }

        private bool isCleanSessionOn = true;
        public bool IsCleanSessionOn
        {
            get => isCleanSessionOn;
            set => SetProperty(ref isCleanSessionOn, value);
        }

        private MqttProtocolVersion protocolVersion = MqttProtocolVersion.V500;
        public MqttProtocolVersion ProtocolVersion
        {
            get => protocolVersion;
            set => SetProperty(ref protocolVersion, value);
        }

        private string receivedMessage;
        public string ReceivedMessage
        {
            get => receivedMessage;
            set => SetProperty(ref receivedMessage, value);
        }

        private string output;
        public string Output
        {
            get => output;
            set => SetProperty(ref output, value);
        }

        private string exceptionText;
        public string ExceptionText
        {
            get => exceptionText;
            set => SetProperty(ref exceptionText, value);
        }

        private IMqttClientController mqttClientController;

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    mqttClient = MqttClientController.MqttClient;
                    clientId = mqttClientController.ClientId;
                }
            }
        }
    }
}
