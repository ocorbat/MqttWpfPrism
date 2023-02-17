namespace MqttServer.Backend.Events
{
    public class ClientUnsubscribedTopicEventArgs
    {
        public ClientUnsubscribedTopicEventArgs(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; }
    }
}
