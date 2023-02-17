using MQTTnet.Server;
using System.Collections.Generic;

namespace MqttServer.Backend.Events
{
    public class ClientConnectedEventArgs
    {
        public ClientConnectedEventArgs(string connectedClient, IList<MqttClientStatus>? currentConnectedClients)
        {
            ConnectedClient = connectedClient;
            CurrentConnectedClients = currentConnectedClients;
        }

        public string ConnectedClient { get; }

        public IList<MqttClientStatus>? CurrentConnectedClients { get; }
    }
}
