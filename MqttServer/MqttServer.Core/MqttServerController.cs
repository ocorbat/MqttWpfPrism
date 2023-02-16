using MQTTnet;
using MQTTnet.Server;
using MqttServer.Core.Interfaces;
using System;

namespace MqttServer.Core
{
    public class MqttServerController : IMqttServerController
    {
        public MqttFactory MqttFactory { get; } = new MqttFactory();
        public MQTTnet.Server.MqttServer MqttServer { get; set; }

        public Guid ClientId { get; }


        public MqttServerController()
        {

        }

        public event EventHandler<EventArgs> ServerStarted;
        public event EventHandler<EventArgs> ServerStopped;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<ClientSubscribedTopicEventArgs> ClientSubscribedTopic;
        public event EventHandler<ClientUnsubscribedTopicEventArgs> ClientUnsubscribedTopic;

        public void OnServerStarted(EventArgs e)
        {
            ServerStarted?.Invoke(this, e);
        }

        public void OnServerStopped(EventArgs e)
        {
            ServerStopped?.Invoke(this, e);
        }

        public void OnClientConnected(ClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        public void OnClientDisconnected(ClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }

        public void OnClientSubscribedTopic(ClientSubscribedTopicEventArgs e)
        {
            ClientSubscribedTopic?.Invoke(this, e);
        }

        public void OnClientUnsubscribedTopic(ClientUnsubscribedTopicEventArgs e)
        {
            ClientUnsubscribedTopic?.Invoke(this, e);
        }
    }
}
