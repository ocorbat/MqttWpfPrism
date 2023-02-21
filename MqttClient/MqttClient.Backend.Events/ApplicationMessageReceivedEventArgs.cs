using System;

namespace MqttClient.Backend.Events
{
    public class ApplicationMessageReceivedEventArgs : EventArgs
    {
        public ApplicationMessageReceivedEventArgs(byte[] applicationMessage, string contentType)
        {
            ApplicationMessage = applicationMessage;
            ContentType = contentType;
        }

        public byte[] ApplicationMessage { get; }

        public string ContentType { get; }
    }
}
