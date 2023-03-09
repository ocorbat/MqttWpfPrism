using Mqtt.Backend.Common;
using Mqtt.Backend.Common.Events;
using Mqtt.Backend.Common.Extensions;
using Mqtt.Backend.Common.Model;
using MqttClient.Backend.Core.Settings;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MqttClient.Backend.Core
{
    public class MqttClientController : IMqttClientController
    {
        private int numberOfInstance;
        private IList<string> listReceivedData = new List<string>();

        public IMqttClient MqttClient { get; private set; }
        public Guid ClientGuid { get; }
        public int ClientId { get; private set; }
        public MqttFactory MqttFactory { get; } = new MqttFactory();

        public ObservableCollection<ClientSubscription> ClientSubscriptions { get; }

        public MqttClientController()
        {
            ClientGuid = Guid.NewGuid();
            MqttClient = MqttFactory.CreateMqttClient();

            ClientSubscriptions = new ObservableCollection<ClientSubscription>();

            MqttClient.ConnectingAsync += MqttClient_ConnectingAsync;
            MqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            MqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
            MqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            MqttClient.InspectPackage += MqttClient_InspectPackage;
        }


        public async Task ConnectAsync(MqttClientConnectSettings settings)
        {
            MqttClientOptions mqttClientOptions = settings.UseWebSockets
                ? MqttFactory.CreateClientOptionsBuilder()
                .WithClientId(ClientId.ToString())
                .WithWebSocketServer($"{Mqtt.Backend.Common.Constants.Localhost}:{settings.PortWebSocket}")
                .WithCleanSession(settings.IsCleanSession)
                .WithSessionExpiryInterval(settings.SessionExpiryInterval)
                .WithKeepAlivePeriod(settings.KeepAlivePeriod)
                .WithProtocolVersion(settings.ProtocolVersion)
                .WithCredentials(settings.Username, settings.Password)
                .WithRequestResponseInformation(settings.ProtocolVersion == MqttProtocolVersion.V500 && settings.RequestResponseInformation)
                .WithRequestProblemInformation(settings.ProtocolVersion == MqttProtocolVersion.V500 && settings.RequestResponseInformation)
                .Build()
                : MqttFactory.CreateClientOptionsBuilder()
                .WithClientId(ClientId.ToString())
                .WithTcpServer(Mqtt.Backend.Common.Constants.Localhost, settings.PortNumber)
                .WithCleanSession(settings.IsCleanSession)
                .WithSessionExpiryInterval(settings.SessionExpiryInterval)
                .WithKeepAlivePeriod(settings.KeepAlivePeriod)
                .WithProtocolVersion(settings.ProtocolVersion)
                .WithCredentials(settings.Username, settings.Password)
                .WithRequestResponseInformation(settings.ProtocolVersion == MqttProtocolVersion.V500 && settings.RequestResponseInformation)
                .WithRequestProblemInformation(settings.ProtocolVersion == MqttProtocolVersion.V500 && settings.RequestResponseInformation)
                .Build();
            try
            {
                MqttClientConnectResult result;
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


        public async Task PublishAsync(string payload, MqttClientPublishSettings settings)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(settings.Topic)
               .WithPayload(payload)
               .WithRetainFlag(settings.IsRetainOn)
               .WithQualityOfServiceLevel(settings.QoS)
               .WithPayloadFormatIndicator(settings.PayloadFormatIndicator)
               .WithContentType(MimeTypes.TextPlain)
               .WithResponseTopic(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.ResponseTopic : default!)
               .WithCorrelationData(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.CorrelationData : default!)
               .WithUserProperty(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.UserProperty.Name : default!, MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.UserProperty.Value : default!)
               .WithMessageExpiryInterval(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.MessageExpiryInterval : default!)
               .Build();

            await PublishAsync(applicationMessage);
        }

        public async Task PublishAsync(byte[] payload, MqttClientPublishSettings settings)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(settings.Topic)
               .WithPayload(payload)
               .WithRetainFlag(settings.IsRetainOn)
               .WithQualityOfServiceLevel(settings.QoS)
               .WithPayloadFormatIndicator(settings.PayloadFormatIndicator)
               .WithContentType(settings.ContentType)
               .WithResponseTopic(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.ResponseTopic : default!)
               .WithCorrelationData(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.CorrelationData : default!)
               .WithUserProperty(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.UserProperty.Name : default!, MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.UserProperty.Value : default!)
               .WithMessageExpiryInterval(MqttClient.Options?.ProtocolVersion == MqttProtocolVersion.V500 ? settings.MessageExpiryInterval : default!)
               .Build();

            await PublishAsync(applicationMessage);
        }

        /// <summary>
        /// Publishes an empty payload for deleting the retained message to the specified topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="isRetainModeOn"></param>
        /// <returns></returns>
        public async Task PublishEmptyAsync(string topic, bool isRetainModeOn = true)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(topic)
               .WithRetainFlag(isRetainModeOn)
               .Build();

            await PublishAsync(applicationMessage);
        }

        public int NumberOfInstance
        {
            get => numberOfInstance;
            set
            {
                numberOfInstance = value;
                ClientId = numberOfInstance;
            }
        }




        public async Task SubscribeAsync(MqttClientSubscribeSettings settings)
        {
            MqttClientSubscribeOptions mqttSubscribeOptions;

            mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(settings.Topic);
                       f.WithQualityOfServiceLevel(settings.QoS);
                       f.WithNoLocal(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 && settings.NoLocalOn);
                       f.WithRetainAsPublished(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 && settings.RetainAsPublishedOn);
                       f.WithRetainHandling(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 ? settings.RetainHandling : MqttRetainHandling.SendAtSubscribe);
                   })
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic($"{settings.Topic}/{ClientId}");
                       f.WithQualityOfServiceLevel(settings.QoS);
                       f.WithNoLocal(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 && settings.NoLocalOn);
                       f.WithRetainAsPublished(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 && settings.RetainAsPublishedOn);
                       f.WithRetainHandling(MqttClient.Options.ProtocolVersion == MqttProtocolVersion.V500 ? settings.RetainHandling : MqttRetainHandling.SendAtSubscribe);
                   })
               .WithSubscriptionIdentifier((uint)ClientId)
               .Build();

            try
            {
                MqttClientSubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientSubscriptionTimeout)))
                {
                    response = await MqttClient.SubscribeAsync(mqttSubscribeOptions, timeoutToken.Token);
                }

                if (response != null)
                {
                    foreach (var item in response.Items)
                    {
                        var resultCode = item.ResultCode;
                        var topicFilter = item.TopicFilter;
                        var existingSubscription = ClientSubscriptions.SingleOrDefault(s => s.TopicFilter.Topic == topicFilter.Topic);

                        if (existingSubscription != null)
                        {
                            ClientSubscriptions.Remove(existingSubscription);
                        }

                        ClientSubscriptions.Add(new ClientSubscription(resultCode, topicFilter));
                    }
                }

                Debug.WriteLine($"MQTT client {MqttClient.Options.ClientId} subscribed to topic '{settings.Topic}'.");
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
            catch (MqttClientUnexpectedDisconnectReceivedException e)
            {
                OnOutputMessage(new OutputMessageEventArgs($"({e})"));
            }
        }

        public async Task UnsubscribeAsync(MqttClientUnsubscribeSettings settings)
        {
            var mqttUnsubscribeOptions = MqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(settings.Topic)
                .WithTopicFilter($"{settings.Topic}/{ClientId}")
                .Build();

            try
            {
                MqttClientUnsubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientUnsubscriptionTimeout)))
                {
                    response = await MqttClient.UnsubscribeAsync(mqttUnsubscribeOptions, timeoutToken.Token);
                }

                if (response != null)
                {
                    foreach (var item in response.Items)
                    {
                        var resultCode = item.ResultCode;
                        if (resultCode == MqttClientUnsubscribeResultCode.Success)
                        {
                            var toto = ClientSubscriptions.SingleOrDefault(s => s.TopicFilter.Topic == item.TopicFilter);
                            if (toto != null)
                            {
                                ClientSubscriptions.Remove(toto);
                            }
                        }
                    }
                }

                Debug.WriteLine($"MQTT client {MqttClient.Options.ClientId} unsubscribed to topic '{settings.Topic}'.");
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
            // Calling _DisconnectAsync_ will send a DISCONNECT packet before closing the connection.
            // Using a reason code requires MQTT version 5.0.0!
            await MqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection);
        }

        private async Task PublishAsync(MqttApplicationMessage mqttApplicationMessage)
        {
            try
            {
                MqttClientPublishResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ClientPublishTimeout)))
                {
                    response = await MqttClient.PublishAsync(mqttApplicationMessage, timeoutToken.Token);
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

        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            OnApplicationMessageReceived(new Events.MqttApplicationMessageReceivedEventArgs(arg));
            return Task.CompletedTask;
        }

        private Task MqttClient_InspectPackage(MQTTnet.Diagnostics.InspectMqttPacketEventArgs arg)
        {
            return Task.CompletedTask;
        }


        public event EventHandler<Events.MqttClientConnectingEventArgs> ClientConnecting = default!;

        public event EventHandler<Events.MqttClientConnectedEventArgs> ClientConnected = default!;

        public event EventHandler<Events.MqttClientDisconnectedEventArgs> ClientDisconnected = default!;

        public event EventHandler<Events.MqttApplicationMessageReceivedEventArgs> ApplicationMessageReceived = default!;

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

        private void OnApplicationMessageReceived(Events.MqttApplicationMessageReceivedEventArgs e)
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

        public bool DeleteRetainedMessagesCommandCanExecute()
        {
            return MqttClient != null && MqttClient.IsConnected;
        }
    }
}
