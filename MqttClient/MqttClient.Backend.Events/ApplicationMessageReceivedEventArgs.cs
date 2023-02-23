using System;

namespace MqttClient.Backend.Events
{
    public class ApplicationMessageReceivedEventArgs : EventArgs
    {
        public ApplicationMessageReceivedEventArgs(string clientId, byte[] applicationMessage, string contentType, byte[] correlationData, string responseTopic)
        {
            ApplicationMessage = applicationMessage;
            ContentType = contentType;
            ClientId = clientId;
            CorrelationData = correlationData;
            ResponseTopic = responseTopic;
        }

        public string ClientId { get; }

        public byte[] ApplicationMessage { get; }

        public string ContentType { get; }

        public byte[] CorrelationData { get; }

        public string ResponseTopic { get; }
    }
}
