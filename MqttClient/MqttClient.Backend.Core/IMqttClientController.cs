using MqttClient.Backend.Core.Settings;
using MqttCommon.Events;
using MQTTnet;
using MQTTnet.Client;

namespace MqttClient.Backend.Core
{
    public interface IMqttClientController
    {
        MqttFactory MqttFactory { get; }
        IMqttClient MqttClient { get; }

        Guid ClientId { get; }

        Task ConnectAsync(MqttClientConnectSettings settings);

        Task DisconnectAsync();


        Task PublishAsync(string payload, MqttClientPublishSettings settings);

        Task PublishAsync(byte[] payload, MqttClientPublishSettings settings);

        Task PublishEmptyAsync(string topic, bool isRetainModeOn = true);


        Task SubscribeAsync(MqttClientSubscribeSettings settings);

        Task UnsubscribeAsync(MqttClientUnsubscribeSettings settings);

        event EventHandler<Events.MqttClientConnectingEventArgs> ClientConnecting;
        event EventHandler<Events.MqttClientConnectedEventArgs> ClientConnected;
        event EventHandler<Events.MqttClientDisconnectedEventArgs> ClientDisconnected;
        event EventHandler<Events.ApplicationMessageReceivedEventArgs> ApplicationMessageReceived;

        event EventHandler<OutputMessageEventArgs> OutputMessage;

        bool PublishCommandCanExecute();

        bool PublishImageCommandCanExecute();

        bool SubscribeCommandCanExecute();

        bool UnsubscribeCommandCanExecute();

        bool ConnectCommandCanExecute();
        bool DisonnectCommandCanExecute();
        bool DeleteRetainedMessagesCommandCanExecute();
    }
}
