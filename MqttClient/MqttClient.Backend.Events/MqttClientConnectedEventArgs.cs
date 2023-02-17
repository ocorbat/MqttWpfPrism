using System;

namespace MqttClient.Backend.Events
{
    public class MqttClientConnectedEventArgs : EventArgs
    {
        public MqttClientConnectedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
