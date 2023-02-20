using MqttCommon;
using MqttCommon.Events;
using MQTTnet;
using MQTTnet.Internal;
using MQTTnet.Packets;
using MQTTnet.Server;
using MqttServer.Backend.Core;
using MqttServer.Backend.Core.Model;
using System.Collections;
using System.Collections.ObjectModel;

namespace MqttServer.Core
{
    public class MqttServerController : IMqttServerController
    {
        public MqttFactory MqttFactory { get; } = new MqttFactory();
        public MQTTnet.Server.MqttServer MqttServer { get; private set; } = default!;

        public Guid ClientId { get; }

        public ObservableCollection<ClientSubscribedItem> ClientSubscribedItems { get; }

        public IList<MqttClientStatus>? ConnectedClients { get; private set; }

        public MqttServerController()
        {
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

        public async Task StopAsync()
        {
            await MqttServer.StopAsync();
        }

        public async Task StartAsync()
        {
            await MqttServer.StartAsync();
        }

        public MQTTnet.Server.MqttServer? CreateServer()
        {
            // Start server
            // The port for the default endpoint is 1883.
            // The default endpoint is NOT encrypted!
            // Use the builder classes where possible.
            var mqttServerOptions = MqttFactory.CreateServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(Constants.Port5004)
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
            }

            return MqttServer;
        }

        private Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            // Here we only change the topic of the received application message.
            // but also changing the payload etc. is required. Changing the QoS after
            // transmitting is not supported and makes no sense at all.
            //args.ApplicationMessage.Topic += "/manipulated";

            return CompletedTask.Instance;
        }

        private Task MqttServer_InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs arg)
        {
            arg.CloseConnection = false;
            return CompletedTask.Instance;
        }

        private Task MqttServer_ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            if (arg.UserName != "admin" || arg.Password != "1234")
            {
                arg.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                return CompletedTask.Instance;
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
            //if (MqttServer != null)
            //{
            try
            {
                ConnectedClients = await MqttServer.GetClientsAsync();
            }
            catch (InvalidOperationException exception)
            {
                OnOutputMessage(new OutputMessageEventArgs(exception.ToString()));
            }
            //}

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
