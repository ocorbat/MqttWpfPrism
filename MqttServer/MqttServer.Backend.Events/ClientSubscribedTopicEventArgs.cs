using System;

namespace MqttServer.Backend.Events
{
    public class ClientSubscribedTopicEventArgs : EventArgs
    {

        public ClientSubscribedTopicEventArgs(string clientId, string topic)
        {
            ClientId = clientId;
            Topic = topic;
        }

        public string ClientId { get; }
        public string Topic { get; }
    }
}
