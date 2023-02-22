using MqttCommon.Events;
using MqttCommon.Extensions;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Server;
using MqttServer.Backend.Core;
using MqttServer.Backend.Core.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Json;
using Constants = MqttServer.Backend.Core.Constants;

namespace MqttServer.Core
{
    public class MqttServerController : IMqttServerController
    {
        public MqttFactory MqttFactory { get; } = new MqttFactory();
        public MQTTnet.Server.MqttServer MqttServer { get; private set; } = default!;
        public ObservableCollection<ClientSubscribedItem> ClientSubscribedItems { get; }
        public IList<MqttClientStatus>? ConnectedClients { get; private set; }

        private readonly string storePath;

        private IList<MqttApplicationMessage>? listRetainedMessages;

        public MqttServerController()
        {
            storePath = Path.Combine(Directory.GetCurrentDirectory(), "RetainedMessages.json");

            ClientSubscribedItems = new ObservableCollection<ClientSubscribedItem>();
            ClientSubscribedItems.CollectionChanged += ClientSubscribedItems_CollectionChanged;
        }

        private void ClientSubscribedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        public event EventHandler<EventArgs> ServerStarted = default!;
        public event EventHandler<EventArgs> ServerStopped = default!;
        public event EventHandler<Backend.Events.ClientConnectedEventArgs> ClientConnected = default!;
        public event EventHandler<Backend.Events.ClientDisconnectedEventArgs> ClientDisconnected = default!;
        public event EventHandler<Backend.Events.ClientSubscribedTopicEventArgs> ClientSubscribedTopic = default!;
        public event EventHandler<Backend.Events.ClientUnsubscribedTopicEventArgs> ClientUnsubscribedTopic = default!;


        public event EventHandler<OutputMessageEventArgs> OutputMessage = default!;

        public async Task<IList<MqttClientStatus>> RefreshConnectedClientsAsync()
        {
            if (MqttServer != null)
            {
                ConnectedClients = await MqttServer.GetClientsAsync();
            }
            return ConnectedClients;
        }

        public bool GetConnectedClientsCommandCanExecute()
        {
            return MqttServer != null && MqttServer.IsStarted;
        }

        private void OnOutputMessage(OutputMessageEventArgs e)
        {
            OutputMessage?.Invoke(this, e);
        }

        public bool StopServerCommandCanExecute()
        {
            return MqttServer != null && MqttServer.IsStarted;
        }

        public bool StartServerCommandCanExecute()
        {
            return MqttServer == null || !MqttServer.IsStarted;
        }

        public bool PublishCommandCanExecute()
        {
            return MqttServer != null && MqttServer.IsStarted;
        }

        public bool DeleteRetainedMessagesCommandCanExecute()
        {
            return true;
        }

        public async Task StopAsync()
        {
            await MqttServer.StopAsync();
        }

        public async Task StartAsync()
        {
            await MqttServer.StartAsync();
        }

        public MQTTnet.Server.MqttServer? CreateServer(MqttServerCreateSettings settings)
        {
            // Start server
            // The port for the default endpoint is 1883.
            // The default endpoint is NOT encrypted!
            // Use the builder classes where possible.
            var mqttServerOptions = MqttFactory.CreateServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(settings.PortNumber)
                .Build();

            MqttServer = MqttFactory.CreateMqttServer(mqttServerOptions);

            if (MqttServer != null)
            {
                MqttServer.StartedAsync += MqttServer_StartedAsync;
                MqttServer.StoppedAsync += MqttServer_StoppedAsync;
                MqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
                MqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
                MqttServer.ClientSubscribedTopicAsync += MqttServer_ClientSubscribedTopicAsync;
                MqttServer.ClientUnsubscribedTopicAsync += MqttServer_ClientUnsubscribedTopicAsync;
                MqttServer.ValidatingConnectionAsync += MqttServer_ValidatingConnectionAsync;
                MqttServer.InterceptingSubscriptionAsync += MqttServer_InterceptingSubscriptionAsync;
                MqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;
                MqttServer.LoadingRetainedMessageAsync += MqttServer_LoadingRetainedMessageAsync;
                MqttServer.RetainedMessageChangedAsync += MqttServer_RetainedMessageChangedAsync;
                MqttServer.RetainedMessagesClearedAsync += MqttServer_RetainedMessagesClearedAsync;
            }

            return MqttServer;
        }

        public async Task PublishAsync(string payload, MqttServerPublishSettings settings)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(settings.Topic)
                .WithPayload(payload)
                .WithRetainFlag(settings.IsRetainOn)
                .WithQualityOfServiceLevel(settings.QoS)
                .WithPayloadFormatIndicator(settings.PayloadFormatIndicator)
                .WithContentType(settings.ContentType)
                .Build();

            InjectedMqttApplicationMessage injectedMqttApplicationMessage = new InjectedMqttApplicationMessage(applicationMessage);

            try
            {
                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ServerPublishTimeout)))
                {
                    await MqttServer.InjectApplicationMessage(injectedMqttApplicationMessage, timeoutToken.Token);
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

        public async Task DeleteRetainedMessagesAsync()
        {
            await MqttServer.DeleteRetainedMessagesAsync();
        }

        private Task MqttServer_RetainedMessagesClearedAsync(EventArgs arg)
        {
            // Make sure to clear the retained messages when they are all deleted via API.
            try
            {
                File.Delete(storePath);
            }
            catch (Exception exception)
            {
                OnOutputMessage(new OutputMessageEventArgs($"{exception.Message}"));
            }

            OnOutputMessage(new OutputMessageEventArgs("Retained messages deleted"));
            return Task.CompletedTask;
        }

        private async Task MqttServer_RetainedMessageChangedAsync(RetainedMessageChangedEventArgs arg)
        {
            // Make sure to persist the changed retained messages.
            try
            {
                // This sample uses the property _StoredRetainedMessages_ which will contain all(!) retained messages.
                // The event args also contain the affected retained message (property ChangedRetainedMessage). This can be
                // used to write all retained messages to dedicated files etc. Then all files must be loaded and a full list
                // of retained messages must be provided in the loaded event.

                listRetainedMessages = arg.StoredRetainedMessages;
                var buffer = JsonSerializer.SerializeToUtf8Bytes(arg.StoredRetainedMessages);
                await File.WriteAllBytesAsync(storePath, buffer);
                OnOutputMessage(new OutputMessageEventArgs("Retained messages saved."));
            }
            catch (Exception exception)
            {
                OnOutputMessage(new OutputMessageEventArgs(exception.ToString()));
            }
        }

        private async Task MqttServer_LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs arg)
        {
            // Make sure that the server will load the retained messages.
            try
            {
                using (FileStream fileStream = File.OpenRead(storePath))
                {
                    arg.LoadedRetainedMessages = await JsonSerializer.DeserializeAsync<List<MqttApplicationMessage>>(fileStream);
                }

                listRetainedMessages = arg.LoadedRetainedMessages;
                OnOutputMessage(new OutputMessageEventArgs("Retained messages loaded."));
                if (arg.LoadedRetainedMessages != null && arg.LoadedRetainedMessages.Any())
                {
                    OnOutputMessage(new OutputMessageEventArgs(arg.LoadedRetainedMessages.DumpToString()));
                }
            }
            catch (FileNotFoundException)
            {
                // Ignore because nothing is stored yet.
                OnOutputMessage(new OutputMessageEventArgs("No retained messages stored yet."));
            }
            catch (Exception exception)
            {
                OnOutputMessage(new OutputMessageEventArgs(exception.ToString()));
            }
        }

        private Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            if (!(arg.SessionItems.Contains("Test")))
            {
                arg.SessionItems.Add("Test", 123);
            }

            // Message with empty payload was sent to delete retained message of corresponding topic
            if ((arg.ApplicationMessage.Payload == null) && (listRetainedMessages?.SingleOrDefault(x => x.Topic == arg.ApplicationMessage.Topic) == null))
            {
                OnOutputMessage(new OutputMessageEventArgs("Kein Topic in den retained messages"));
            }

            // Here we only change the topic of the received application message.
            // but also changing the payload etc. is required. Changing the QoS after
            // transmitting is not supported and makes no sense at all.
            //args.ApplicationMessage.Topic += "/manipulated";

            //var payload = arg.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(arg.ApplicationMessage?.Payload);


            //WriteLine(
            //    " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

            //    DateTime.Now,
            //    arg.ClientId,
            //    arg.ApplicationMessage?.Topic,
            //    payload,
            //    arg.ApplicationMessage?.QualityOfServiceLevel,
            //    arg.ApplicationMessage?.Retain);

            return Task.CompletedTask;
        }

        private Task MqttServer_InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs arg)
        {
            if (!(arg.SessionItems.Contains("SomeData")))
            {
                arg.SessionItems.Add("SomeData", true);
            }

            arg.CloseConnection = false;
            return Task.CompletedTask;
        }

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {

            if (!(arg.SessionItems.Contains(arg.ClientId)))
            {
                arg.SessionItems.Add(arg.ClientId, true);
            }

            if (arg.UserName != "admin" || arg.Password != "1234")
            {
                arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                return Task.CompletedTask;
            }

            arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
            return Task.CompletedTask;
        }

        private Task MqttServer_ClientUnsubscribedTopicAsync(ClientUnsubscribedTopicEventArgs arg)
        {
            var clientId = arg.ClientId;
            string toto = arg.TopicFilter;
            IDictionary sessionItems = arg.SessionItems;

            OnClientUnsubscribedTopic(new Backend.Events.ClientUnsubscribedTopicEventArgs(arg.ClientId));
            return Task.CompletedTask;
        }

        private Task MqttServer_ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            var clientId = arg.ClientId;
            MqttTopicFilter toto = arg.TopicFilter;
            IDictionary sessionItems = arg.SessionItems;

            ClientSubscribedItems.Add(new ClientSubscribedItem(arg.ClientId, arg.TopicFilter, arg.SessionItems));
            OnClientSubscribedTopic(new Backend.Events.ClientSubscribedTopicEventArgs(arg.ClientId, arg.TopicFilter.Topic));

            return Task.CompletedTask;
        }

        private async Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            await OnClientDisconnectedAsync(arg);
        }

        private async Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            await OnClientConnectedAsync(arg);
        }

        private Task MqttServer_StoppedAsync(EventArgs arg)
        {
            OnServerStopped(new EventArgs());
            return Task.CompletedTask;
        }

        private Task MqttServer_StartedAsync(EventArgs arg)
        {
            OnServerStarted(new EventArgs());
            return Task.CompletedTask;
        }

        private void OnServerStarted(EventArgs e)
        {
            ServerStarted?.Invoke(this, e);
        }

        private void OnServerStopped(EventArgs e)
        {
            ServerStopped?.Invoke(this, e);
        }

        private async Task OnClientConnectedAsync(ClientConnectedEventArgs e)
        {
            if (MqttServer != null)
            {
                ConnectedClients = await MqttServer.GetClientsAsync();
            }

            ClientConnected?.Invoke(this, new Backend.Events.ClientConnectedEventArgs(e.ClientId, ConnectedClients));
        }

        private async Task OnClientDisconnectedAsync(ClientDisconnectedEventArgs e)
        {
            try
            {
                ConnectedClients = await MqttServer.GetClientsAsync();
            }
            catch (InvalidOperationException exception)
            {
                OnOutputMessage(new OutputMessageEventArgs(exception.ToString()));
            }

            ClientDisconnected?.Invoke(this, new Backend.Events.ClientDisconnectedEventArgs(e.ClientId, ConnectedClients));
        }

        private void OnClientSubscribedTopic(Backend.Events.ClientSubscribedTopicEventArgs e)
        {
            ClientSubscribedTopic?.Invoke(this, e);
        }

        private void OnClientUnsubscribedTopic(Backend.Events.ClientUnsubscribedTopicEventArgs e)
        {
            ClientUnsubscribedTopic?.Invoke(this, e);
        }


    }
}
