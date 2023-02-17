using MqttCommon.Events;
using MQTTnet;
using MQTTnet.Server;

namespace MqttServer.Backend.Core
{
    public interface IMqttServerController
    {
        MqttFactory MqttFactory { get; }
        MQTTnet.Server.MqttServer MqttServer { get; }

        IList<MqttClientStatus>? ConnectedClients { get; }

        Guid ClientId { get; }

        MQTTnet.Server.MqttServer? CreateServer();
        Task<IList<MqttClientStatus>> RefreshConnectedClientsAsync();

        bool GetConnectedClientsCommandCanExecute();


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
    }
}
