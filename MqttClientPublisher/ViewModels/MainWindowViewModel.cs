using MqttCommon;
using MqttCommon.Extensions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttClientPublisher.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private MqttFactory mqttFactory = new MqttFactory();
        private IMqttClient? mqttClient;
        private Guid clientId = Guid.NewGuid();
        private string _title = "MQTT Client Publisher";
        private string status = string.Empty;
        private string sendMessageText = "Enter Message";
        private string exceptionText;

        public MainWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
        }

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }
        public DelegateCommand PublishCommand { get; set; }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public string ExceptionText
        {
            get => exceptionText;
            set => SetProperty(ref exceptionText, value);
        }

        public string SendMessageText
        {
            get => sendMessageText;
            set => SetProperty(ref sendMessageText, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool PublishCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void PublishCommandExecute()
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(CurrentTopic)
               .WithPayload(SendMessageText)
               .WithRetainFlag(IsRetainModeOn)
               .WithQualityOfServiceLevel(QualityOfServiceLevel)
               .Build();

            try
            {
                MqttClientPublishResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                {
                    response = await mqttClient.PublishAsync(applicationMessage, timeoutToken.Token);

                    if (response.IsSuccess)
                    {
                        Output = response.DumpToString();
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                ExceptionText = $"({e})";
                Debug.WriteLine($"Timeout while publishing. {e}");
            }
            catch (MQTTnet.Exceptions.MqttCommunicationTimedOutException e)
            {
                ExceptionText = $"({e})";
                Debug.WriteLine($"Timeout while publishing. {e}");
            }
        }

        private bool DisonnectCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void DisonnectCommandExecute()
        {
            await mqttClient.DisconnectAsync();
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
                .WithCleanSession(true)
                .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
                .WithProtocolVersion(ProtocolVersion)
                .WithCredentials(Username, Password)
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                Debug.WriteLine("Received application message.");
                stringBuilder.AppendLine(e.DumpToString());

                var payloadString = Convert.ToString(e.ApplicationMessage.Payload);

                // Convert Payload to string
                var payload = e.ApplicationMessage?.Payload == null ? null : System.Text.Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);

                stringBuilder.AppendLine(payload);
                Output = stringBuilder.ToString();

                return Task.CompletedTask;
            };

            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            mqttClient.ConnectingAsync += MqttClient_ConnectingAsync;

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

        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Status = $"Client {clientId} is connecting";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Status = $"Client {clientId} is connected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
            Output = string.Empty;
            ExceptionText = string.Empty;
            return Task.CompletedTask;
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Status = $"Client {clientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
            Output = string.Empty;
            ExceptionText = string.Empty;
            return Task.CompletedTask;
        }

        private bool isRetainModeOn = false;
        public bool IsRetainModeOn
        {
            get => isRetainModeOn;
            set => SetProperty(ref isRetainModeOn, value);
        }

        private string currentTopic = "Topic1";
        public string CurrentTopic
        {
            get => currentTopic;
            set => SetProperty(ref currentTopic, value);
        }

        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
        }

        private bool isCleanSessionOn = true;
        public bool IsCleanSessionOn
        {
            get => isCleanSessionOn;
            set => SetProperty(ref isCleanSessionOn, value);
        }

        private string output;
        public string Output
        {
            get => output;
            set => SetProperty(ref output, value);
        }

        private string password = "1234";
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private string username = "admin";
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private MqttProtocolVersion protocolVersion = MqttProtocolVersion.V500;
        public MqttProtocolVersion ProtocolVersion
        {
            get => protocolVersion;
            set => SetProperty(ref protocolVersion, value);
        }

    }
}
