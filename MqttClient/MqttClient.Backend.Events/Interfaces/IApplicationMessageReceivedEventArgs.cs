namespace MqttClient.Backend.Events.Interfaces
{
    public interface IApplicationMessageReceivedEventArgs
    {
        string ClientId { get; }
        string ContentType { get; }
        byte[] CorrelationData { get; }
        uint MessageExpiryInterval { get; }
        byte[] Payload { get; }

        string ResponseTopic { get; }

        bool Retain { get; }
        string Topic { get; }
        ushort TopicAlias { get; }
    }
}
