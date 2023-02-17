using System;

namespace MqttClient.Backend.Events
{
    public class MqttClientDisconnectedEventArgs : EventArgs
    {
        public MqttClientDisconnectedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
