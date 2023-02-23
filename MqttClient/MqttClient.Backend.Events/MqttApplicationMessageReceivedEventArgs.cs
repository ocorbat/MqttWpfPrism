using MqttClient.Backend.Events.Interfaces;

namespace MqttClient.Backend.Events
{
    public class MqttApplicationMessageReceivedEventArgs : IApplicationMessageReceivedEventArgs
    {
        public MqttApplicationMessageReceivedEventArgs(MQTTnet.Client.MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            ClientId = eventArgs.ClientId;
            Payload = eventArgs.ApplicationMessage.Payload;
            ContentType = eventArgs.ApplicationMessage.ContentType;
            CorrelationData = eventArgs.ApplicationMessage.CorrelationData;
            ResponseTopic = eventArgs.ApplicationMessage.ResponseTopic;
            MessageExpiryInterval = eventArgs.ApplicationMessage.MessageExpiryInterval;
            Retain = eventArgs.ApplicationMessage.Retain;
            Topic = eventArgs.ApplicationMessage.Topic;
            TopicAlias = eventArgs.ApplicationMessage.TopicAlias;
        }

        public string ClientId { get; private set; }
        public byte[] Payload { get; private set; }
        public string ContentType { get; private set; }

        public byte[] CorrelationData { get; private set; }
        public string ResponseTopic { get; private set; }

        public uint MessageExpiryInterval { get; private set; }

        public bool Retain { get; private set; }

        public string Topic { get; private set; }

        public ushort TopicAlias { get; private set; }
    }
}
