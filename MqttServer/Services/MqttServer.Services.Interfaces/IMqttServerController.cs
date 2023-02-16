using MQTTnet;
using MQTTnet.Server;
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
        event EventHandler<ClientConnectedEventArgs> ClientConnected;
        event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        event EventHandler<ClientSubscribedTopicEventArgs> ClientSubscribedTopic;
        event EventHandler<ClientUnsubscribedTopicEventArgs> ClientUnsubscribedTopic;
        //event EventHandler<MqttClientDisconnectedEventArgs> ClientDisconnected;

        //event EventHandler<OutputMessageEventArgs> OutputMessage;

        void OnServerStarted(EventArgs e);
        void OnServerStopped(EventArgs e);
        void OnClientConnected(ClientConnectedEventArgs e);
        void OnClientDisconnected(ClientDisconnectedEventArgs e);

        void OnClientSubscribedTopic(ClientSubscribedTopicEventArgs e);
        void OnClientUnsubscribedTopic(ClientUnsubscribedTopicEventArgs e);




        //void OnOutputMessage(OutputMessageEventArgs e);
    }
}
