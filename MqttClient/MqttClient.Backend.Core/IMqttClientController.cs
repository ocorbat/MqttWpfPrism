﻿using MqttCommon.Events;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;

namespace MqttClient.Backend.Core
{
    public interface IMqttClientController
    {
        MqttFactory MqttFactory { get; }
        IMqttClient MqttClient { get; }

        Guid ClientId { get; }

        Task ConnectAsync(bool isCleanSessionOn, MqttProtocolVersion protocolVersion, string username, string password);

        Task DisconnectAsync();


        Task PublishAsync(string topic, string payload, bool isRetainModeOn, MqttQualityOfServiceLevel qualityOfServiceLevel);


        Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qualityOfServiceLevel, bool isNoLocalOn, bool isRetainAsPublishedOn, MqttRetainHandling retainHandling);
        Task UnsubscribeAsync(string topic);


        event EventHandler<Events.MqttClientConnectingEventArgs> ClientConnecting;
        event EventHandler<Events.MqttClientConnectedEventArgs> ClientConnected;
        event EventHandler<Events.MqttClientDisconnectedEventArgs> ClientDisconnected;
        event EventHandler<Events.ApplicationMessageReceivedEventArgs> ApplicationMessageReceived;

        event EventHandler<OutputMessageEventArgs> OutputMessage;

        bool PublishCommandCanExecute();

        bool SubscribeCommandCanExecute();

        bool UnsubscribeCommandCanExecute();

        bool ConnectCommandCanExecute();
        bool DisonnectCommandCanExecute();
    }
}
