﻿using MQTTnet.Server;
using System;
using System.Collections.Generic;

namespace MqttServer.Backend.Events
{
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientDisconnectedEventArgs(string disconnectedClient, IList<MqttClientStatus>? currentConnectedClients)
        {
            DisconnectedClient = disconnectedClient;
            CurrentConnectedClients = currentConnectedClients;
        }

        public string DisconnectedClient { get; }

        public IList<MqttClientStatus>? CurrentConnectedClients { get; }
    }
}
