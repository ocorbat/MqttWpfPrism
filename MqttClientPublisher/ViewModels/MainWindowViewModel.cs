using MqttCommon;
using MqttCommon.Extensions;
using MQTTnet;
using MQTTnet.Client;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
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
        private string status;
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
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public string ExceptionText
        {
            get { return exceptionText; }
            set { SetProperty(ref exceptionText, value); }
        }

        public string SendMessageText
        {
            get { return sendMessageText; }
            set { SetProperty(ref sendMessageText, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool PublishCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void PublishCommandExecute()
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic("topic1")
               .WithPayload(SendMessageText)
               .Build();

            try
            {
                MqttClientPublishResult response1;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                {
                    response1 = await mqttClient.PublishAsync(applicationMessage, timeoutToken.Token);

                    if (response1.IsSuccess)
                    {
                        ExceptionText = response1.DumpToString();
                    }

                    Debug.WriteLine($"Message published {applicationMessage.Payload}");
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
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Debug.WriteLine("Received application message.");
                e.DumpToConsole();

                var payloadString = Convert.ToString(e.ApplicationMessage.Payload);

                // Convert Payload to string
                var payload = e.ApplicationMessage?.Payload == null ? null : System.Text.Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);


                Console.WriteLine(payload);

                return Task.CompletedTask;
            };

            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            mqttClient.ConnectingAsync += MqttClient_ConnectingAsync;

            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
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
            return Task.CompletedTask;
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Status = $"Client {clientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }





    }
}
