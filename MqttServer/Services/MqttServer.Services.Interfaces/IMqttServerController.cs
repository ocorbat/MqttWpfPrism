using MQTTnet;
using System;

namespace MqttServer.Services.Interfaces
{
    public interface IMqttServerController
    {
        MqttFactory MqttFactory { get; }
        MQTTnet.Server.MqttServer MqttServer { get; }

        Guid ClientId { get; }


        //event EventHandler<MqttClientConnectingEventArgs> ClientConnecting;
        //event EventHandler<MqttClientConnectedEventArgs> ClientConnected;
        //event EventHandler<MqttClientDisconnectedEventArgs> ClientDisconnected;

        //event EventHandler<OutputMessageEventArgs> OutputMessage;

        //void OnClientConnecting(MqttClientConnectingEventArgs e);
        //void OnClientConnected(MqttClientConnectedEventArgs e);
        //void OnClientDisconnected(MqttClientDisconnectedEventArgs e);


        //void OnOutputMessage(OutputMessageEventArgs e);
    }
}
