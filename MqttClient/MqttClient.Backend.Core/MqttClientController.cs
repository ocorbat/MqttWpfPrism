using MqttClient.Backend.Events;
using MQTTnet;
using MQTTnet.Client;

namespace MqttClient.Backend.Core
{
    public class MqttClientController : IMqttClientController
    {
        public IMqttClient MqttClient { get; private set; }
        public Guid ClientId { get; private set; }

        public MqttFactory MqttFactory { get; } = new MqttFactory();

        public MqttClientController()
        {
            MqttClient = MqttFactory.CreateMqttClient();
            ClientId = Guid.NewGuid();
        }

        public event EventHandler<MqttClientConnectingEventArgs> ClientConnecting;

        public event EventHandler<MqttClientConnectedEventArgs> ClientConnected;

        public event EventHandler<MqttClientDisconnectedEventArgs> ClientDisconnected;

        public event EventHandler<OutputMessageEventArgs> OutputMessage;

        public void OnClientConnecting(MqttClientConnectingEventArgs e)
        {
            ClientConnecting?.Invoke(this, e);
        }

        public void OnClientConnected(MqttClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        public void OnClientDisconnected(MqttClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }

        public void OnOutputMessage(OutputMessageEventArgs e)
        {
            OutputMessage?.Invoke(this, e);
        }

    }
}
