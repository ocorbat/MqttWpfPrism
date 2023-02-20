﻿using MqttCommon.Events;
using MqttCommon.Extensions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System.Diagnostics;

namespace MqttClient.Backend.Core
{
    public class MqttClientController : IMqttClientController
    {
        public IMqttClient MqttClient { get; private set; }
        public Guid ClientId { get; private set; }

        public MqttFactory MqttFactory { get; } = new MqttFactory();

        private IList<string> listReceivedData = new List<string>();

        public MqttClientController()
        {
            MqttClient = MqttFactory.CreateMqttClient();

            MqttClient.ConnectingAsync += MqttClient_ConnectingAsync;
            MqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            MqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;

            ClientId = Guid.NewGuid();
        }

        public async Task ConnectAsync(bool isCleanSessionOn, MqttProtocolVersion protocolVersion, string username, string password)
        {
            var mqttClientOptions = MqttFactory.CreateClientOptionsBuilder()
                .WithClientId(ClientId.ToString())
                .WithTcpServer(MqttCommon.Constants.Localhost, MqttCommon.Constants.Port5004)
                .WithCleanSession(isCleanSessionOn)
                .WithKeepAlivePeriod(new TimeSpan(0, 1, 0))
                .WithProtocolVersion(protocolVersion)
                .WithCredentials(username, password)
                .Build();

            MqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine("Received application message.");
                e.DumpToConsole();

                var payloadString = Convert.ToString(e.ApplicationMessage.Payload);

                // Convert Payload to string
                var payload = e.ApplicationMessage?.Payload == null ? null : System.Text.Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);

                if (payload != null)
                {
                    listReceivedData.Add(payload);
                    OnApplicationMessageReceived(new Events.ApplicationMessageReceivedEventArgs(payload));
                }

                Debug.WriteLine(payload);

                return Task.CompletedTask;
            };

            MqttClientConnectResult result;
            try
            {
                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientConnectionTimeout)))
                {
                    result = await MqttClient.ConnectAsync(mqttClientOptions, timeoutToken.Token);
                }

                OnOutputMessage(new OutputMessageEventArgs(result.DumpToString()));
            }
            catch (Exception e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
        }


        public async Task PublishAsync(string topic, string payload, bool isRetainModeOn, MqttQualityOfServiceLevel qualityOfServiceLevel)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(topic)
               .WithPayload(payload)
               .WithRetainFlag(isRetainModeOn)
               .WithQualityOfServiceLevel(qualityOfServiceLevel)
               .Build();

            try
            {
                MqttClientPublishResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientPublishTimeout)))
                {
                    response = await MqttClient.PublishAsync(applicationMessage, timeoutToken.Token);
                }

                if (response.IsSuccess)
                {
                    OnOutputMessage(new OutputMessageEventArgs(response.DumpToString()));
                }
            }
            catch (OperationCanceledException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
            catch (MQTTnet.Exceptions.MqttCommunicationTimedOutException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
        }


        public async Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qualityOfServiceLevel, bool isNoLocalOn, bool isRetainAsPublishedOn, MqttRetainHandling retainHandling)
        {
            MqttClientSubscribeOptions mqttSubscribeOptions;

            mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(topic).WithAtMostOnceQoS();
                       f.WithQualityOfServiceLevel(qualityOfServiceLevel);
                       f.WithNoLocal(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 && isNoLocalOn);
                       f.WithRetainAsPublished(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 && isRetainAsPublishedOn);
                       f.WithRetainHandling(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 ? retainHandling : MqttRetainHandling.SendAtSubscribe);
                   })
               .Build();

            try
            {
                MqttClientSubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientSubscriptionTimeout)))
                {
                    response = await MqttClient.SubscribeAsync(mqttSubscribeOptions, timeoutToken.Token);
                }

                Debug.WriteLine($"MQTT client {MqttClient.Options.ClientId} subscribed to topic '{topic}'.");
                // The response contains additional data sent by the server after subscribing.
                OnOutputMessage(new OutputMessageEventArgs(response.DumpToString()));
            }
            catch (OperationCanceledException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
            catch (MQTTnet.Exceptions.MqttCommunicationTimedOutException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
        }


        public async Task UnsubscribeAsync(string topic)
        {
            MqttClientUnsubscribeResult result;
            var mqttUnsubscribeOptions = MqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            try
            {
                MqttClientUnsubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientUnsubscriptionTimeout)))
                {
                    response = await MqttClient.UnsubscribeAsync(mqttUnsubscribeOptions, timeoutToken.Token);
                }

                Debug.WriteLine($"MQTT client {MqttClient.Options.ClientId} unsubscribed to topic '{topic}'.");
                // The response contains additional data sent by the server after subscribing.
                OnOutputMessage(new OutputMessageEventArgs(response.DumpToString()));
            }
            catch (OperationCanceledException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
            catch (MQTTnet.Exceptions.MqttCommunicationTimedOutException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
        }

        public async Task DisconnectAsync()
        {
            await MqttClient.DisconnectAsync();
        }


        private Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            OnClientDisconnected(new Events.MqttClientDisconnectedEventArgs(arg.ConnectResult.DumpToString()));
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            OnClientConnected(new Events.MqttClientConnectedEventArgs(arg.ConnectResult.DumpToString()));
            return Task.CompletedTask;
        }

        private Task MqttClient_ConnectingAsync(MqttClientConnectingEventArgs arg)
        {
            OnClientConnecting(new Events.MqttClientConnectingEventArgs(arg.ClientOptions.DumpToString()));
            return Task.CompletedTask;
        }

        public event EventHandler<Events.MqttClientConnectingEventArgs> ClientConnecting = default!;

        public event EventHandler<Events.MqttClientConnectedEventArgs> ClientConnected = default!;

        public event EventHandler<Events.MqttClientDisconnectedEventArgs> ClientDisconnected = default!;

        public event EventHandler<Events.ApplicationMessageReceivedEventArgs> ApplicationMessageReceived = default!;

        public event EventHandler<OutputMessageEventArgs> OutputMessage = default!;

        private void OnClientConnecting(Events.MqttClientConnectingEventArgs e)
        {
            ClientConnecting?.Invoke(this, e);
        }

        private void OnClientConnected(Events.MqttClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        private void OnClientDisconnected(Events.MqttClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }

        private void OnApplicationMessageReceived(Events.ApplicationMessageReceivedEventArgs e)
        {
            ApplicationMessageReceived?.Invoke(this, e);
        }

        private void OnOutputMessage(OutputMessageEventArgs e)
        {
            OutputMessage?.Invoke(this, e);
        }

        public bool ConnectCommandCanExecute()
        {
            return MqttClient == null || !MqttClient.IsConnected;
        }

        public bool DisonnectCommandCanExecute()
        {
            return MqttClient == null || MqttClient.IsConnected;
        }

        public bool PublishCommandCanExecute()
        {
            return MqttClient != null && MqttClient.IsConnected;
        }

        public bool SubscribeCommandCanExecute()
        {
            return MqttClient != null && MqttClient.IsConnected;
        }

        public bool UnsubscribeCommandCanExecute()
        {
            return MqttClient != null && MqttClient.IsConnected;
        }


    }
}