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

namespace MqttClientSubscriber.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private MqttFactory mqttFactory = new MqttFactory();
        private IMqttClient? mqttClient;
        private Guid clientId = Guid.NewGuid();

        private string _title = "MQTT Client Subscriber";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
            SubscribeCommand = new DelegateCommand(SubscribeCommandExecute, SubscribeCommandCanExecute);
            UnsubscribeCommand = new DelegateCommand(UnsubscribeCommandExecute, UnsubscribeCommandCanExecute);
        }

        private bool UnsubscribeCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void UnsubscribeCommandExecute()
        {
            var mqttUnsubscribeOptions = mqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter("topic1")
                .Build();

            await mqttClient.UnsubscribeAsync(mqttUnsubscribeOptions);
        }

        private bool SubscribeCommandCanExecute()
        {
            return mqttClient == null ? false : mqttClient.IsConnected;
        }

        private async void SubscribeCommandExecute()
        {
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic("topic1");
                   })
               .Build();

            try
            {
                MqttClientSubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, timeoutToken.Token);
                }

                //Console.ReadLine();
                Console.WriteLine("MQTT client subscribed to topic.");
                // The response contains additional data sent by the server after subscribing.
                response.DumpToConsole();
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
                .WithCleanSession(true)
                .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
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
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Status = $"Client {clientId} is connected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public DelegateCommand ConnectCommand { get; set; }

        public DelegateCommand DisconnectCommand { get; set; }

        public DelegateCommand SubscribeCommand { get; set; }

        public DelegateCommand UnsubscribeCommand { get; set; }



        private string receivedMessage;
        public string ReceivedMessage
        {
            get { return receivedMessage; }
            set { SetProperty(ref receivedMessage, value); }
        }

        private string exceptionText;
        public string ExceptionText
        {
            get { return exceptionText; }
            set { SetProperty(ref exceptionText, value); }
        }
    }
}
