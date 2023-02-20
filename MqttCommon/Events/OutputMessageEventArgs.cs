﻿namespace MqttCommon.Events
{
    public class OutputMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public OutputMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}