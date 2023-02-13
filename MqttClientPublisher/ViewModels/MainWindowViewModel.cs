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
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
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
            if (mqttClient.IsConnected)
            {
                Status = $"Client {clientId} is connected";
            }
            else
            {
                Status = $"Client {clientId} is disconnected";
            }
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
        }

        private bool ConnectCommandCanExecute()
        {
            return mqttClient == null ? true : !mqttClient.IsConnected;
        }

        private async void ConnectCommandExecute()
        {
            mqttClient = mqttFactory.CreateMqttClient();

            // var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(Constants.Localhost, Constants.PortNumber).Build();

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

            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            }
            catch (Exception e)
            {
                ExceptionText = $"({e})";
                Debug.WriteLine($"Timeout while publishing. {e}");
            }

            if (mqttClient.IsConnected)
            {
                Status = $"Client {clientId} is connected";
            }
            else
            {
                Status = $"Client {clientId} is disconnected";
            }

            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            PublishCommand.RaiseCanExecuteChanged();
        }

        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        public DelegateCommand ConnectCommand { get; set; }

        public DelegateCommand DisconnectCommand { get; set; }

        public DelegateCommand PublishCommand { get; set; }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private string exceptionText;
        public string ExceptionText
        {
            get { return exceptionText; }
            set { SetProperty(ref exceptionText, value); }
        }

        private string sendMessageText = "Enter Message";
        public string SendMessageText
        {
            get { return sendMessageText; }
            set { SetProperty(ref sendMessageText, value); }
        }
    }
}
