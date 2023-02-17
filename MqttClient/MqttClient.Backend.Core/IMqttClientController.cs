using MqttClient.Backend.Events;
using MQTTnet;
using MQTTnet.Client;

namespace MqttClient.Backend.Core
{
    public interface IMqttClientController
    {
        MqttFactory MqttFactory { get; }
        IMqttClient MqttClient { get; }

        Guid ClientId { get; }


        event EventHandler<MqttClientConnectingEventArgs> ClientConnecting;
        event EventHandler<MqttClientConnectedEventArgs> ClientConnected;
        event EventHandler<MqttClientDisconnectedEventArgs> ClientDisconnected;

        event EventHandler<OutputMessageEventArgs> OutputMessage;

        void OnClientConnecting(MqttClientConnectingEventArgs e);
        void OnClientConnected(MqttClientConnectedEventArgs e);
        void OnClientDisconnected(MqttClientDisconnectedEventArgs e);


        void OnOutputMessage(OutputMessageEventArgs e);
    }
}
