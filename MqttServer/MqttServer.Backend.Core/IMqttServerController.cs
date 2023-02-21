using MqttCommon.Events;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MqttServer.Backend.Core
{
    public interface IMqttServerController
    {
        MqttFactory MqttFactory { get; }
        MQTTnet.Server.MqttServer MqttServer { get; }
        IList<MqttClientStatus>? ConnectedClients { get; }
        MQTTnet.Server.MqttServer? CreateServer(int portNumber);
        Task<IList<MqttClientStatus>> RefreshConnectedClientsAsync();

        Task PublishAsync(string topic, string payload, bool isRetainModeOn, MqttQualityOfServiceLevel qualityOfServiceLevel);

        event EventHandler<EventArgs> ServerStarted;
        event EventHandler<EventArgs> ServerStopped;
        event EventHandler<Events.ClientConnectedEventArgs> ClientConnected;
        event EventHandler<Events.ClientDisconnectedEventArgs> ClientDisconnected;
        event EventHandler<Events.ClientSubscribedTopicEventArgs> ClientSubscribedTopic;
        event EventHandler<Events.ClientUnsubscribedTopicEventArgs> ClientUnsubscribedTopic;
        event EventHandler<OutputMessageEventArgs> OutputMessage;

        Task StopAsync();
        Task StartAsync();

        bool StopServerCommandCanExecute();
        bool StartServerCommandCanExecute();
        bool GetConnectedClientsCommandCanExecute();
        bool PublishCommandCanExecute();
    }
}
