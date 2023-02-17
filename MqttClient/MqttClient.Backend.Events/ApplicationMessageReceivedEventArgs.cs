using System;

namespace MqttClient.Backend.Events
{
    public class ApplicationMessageReceivedEventArgs : EventArgs
    {
        public ApplicationMessageReceivedEventArgs(string applicationMessage)
        {
            ApplicationMessage = applicationMessage;
        }

        public string ApplicationMessage { get; }
    }
}
