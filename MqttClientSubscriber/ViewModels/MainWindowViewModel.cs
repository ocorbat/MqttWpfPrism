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
using System.Threading;
using System.Threading.Tasks;

namespace MqttClientSubscriber.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private MqttFactory mqttFactory = new MqttFactory();
        private IMqttClient? mqttClient;
        private Guid clientId = Guid.NewGuid();

        private string _title = "MQTT Client Subscriber";
        private string status = string.Empty;

        private string receivedMessage;
        private string exceptionText;
        public MainWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
            SubscribeCommand = new DelegateCommand(SubscribeCommandExecute, SubscribeCommandCanExecute);
            UnsubscribeCommand = new DelegateCommand(UnsubscribeCommandExecute, UnsubscribeCommandCanExecute);
        }

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }
        public DelegateCommand SubscribeCommand { get; set; }
        public DelegateCommand UnsubscribeCommand { get; set; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public string ReceivedMessage
        {
            get => receivedMessage;
            set => SetProperty(ref receivedMessage, value);
        }

        public string ExceptionText
        {
            get => exceptionText;
            set => SetProperty(ref exceptionText, value);
        }

        private string output;
        public string Output
        {
            get => output;
            set => SetProperty(ref output, value);
        }

        private bool UnsubscribeCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void UnsubscribeCommandExecute()
        {
            MqttClientUnsubscribeResult result;
            var mqttUnsubscribeOptions = mqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(CurrentTopic)
                .Build();

            result = await mqttClient.UnsubscribeAsync(mqttUnsubscribeOptions);
            Output = result.DumpToString();
        }

        private bool SubscribeCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void SubscribeCommandExecute()
        {
            MqttClientSubscribeOptions mqttSubscribeOptions;

            switch (QualityOfServiceLevel)
            {
                case MqttQualityOfServiceLevel.AtMostOnce:
                default:
                    mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(CurrentTopic).WithAtMostOnceQoS();
                   })
               .Build();
                    break;
                case MqttQualityOfServiceLevel.AtLeastOnce:
                    mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(CurrentTopic).WithAtLeastOnceQoS();
                   })
               .Build();

                    break;
                case MqttQualityOfServiceLevel.ExactlyOnce:
                    mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(CurrentTopic).WithExactlyOnceQoS();
                   })
               .Build();
                    break;
            }

            try
            {
                MqttClientSubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, timeoutToken.Token);
                }

                Debug.WriteLine($"MQTT client {mqttClient.Options.ClientId} subscribed to topic '{CurrentTopic}'.");
                // The response contains additional data sent by the server after subscribing.
                Output = response.DumpToString();
            }
            catch (OperationCanceledException e)
            {
                ExceptionText = $"({e})";
                Debug.WriteLine($"Timeout while publishing. {e}");
            }
            catch (MQTTnet.Exceptions.MqttCommunicationTimedOutException e)
            {
                ExceptionText = $"({e})";
                Debug.WriteLine(value: e.ToString());
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

        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            Status = $"Client {clientId} is connecting";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Status = $"Client {clientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
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
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            ReceivedMessage = string.Empty;
            Output = string.Empty;
            ExceptionText = string.Empty;
            return Task.CompletedTask;
        }

        private string currentTopic = "Topic1";
        public string CurrentTopic
        {
            get => currentTopic;
            set => SetProperty(ref currentTopic, value);
        }

        private bool isCleanSessionOn = true;
        public bool IsCleanSessionOn
        {
            get => isCleanSessionOn;
            set => SetProperty(ref isCleanSessionOn, value);
        }

        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
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
