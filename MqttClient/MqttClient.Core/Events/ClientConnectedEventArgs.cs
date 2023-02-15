using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient.Core.Events
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public ClientConnectedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
