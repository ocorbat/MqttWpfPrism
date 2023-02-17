using System;

namespace MqttServer.Backend.Events
{
    public class ClientSubscribedTopicEventArgs : EventArgs
    {

        public ClientSubscribedTopicEventArgs(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; }
    }
}
