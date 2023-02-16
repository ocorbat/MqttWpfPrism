using MQTTnet;
using System;

namespace MqttServer.Services.Interfaces
{
    public interface IMqttServerController
    {
        MqttFactory MqttFactory { get; }
        MQTTnet.Server.MqttServer MqttServer { get; set; }

        Guid ClientId { get; }


        event EventHandler<EventArgs> ServerStarted;
        event EventHandler<EventArgs> ServerStopped;
        //event EventHandler<MqttClientDisconnectedEventArgs> ClientDisconnected;

        //event EventHandler<OutputMessageEventArgs> OutputMessage;

        void OnServerStarted(EventArgs e);
        void OnServerStopped(EventArgs e);



        //void OnOutputMessage(OutputMessageEventArgs e);
    }
}
