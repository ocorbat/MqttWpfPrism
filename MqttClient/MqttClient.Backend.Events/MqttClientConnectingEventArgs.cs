using System;

namespace MqttClient.Backend.Events
{
    public class MqttClientConnectingEventArgs : EventArgs
    {
        public MqttClientConnectingEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
