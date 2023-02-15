using MQTTnet.Client;
using System;

namespace MqttClient.Services.Interfaces
{
    public interface IMqttClientController
    {
        IMqttClient MqttClient { get; set; }

        Guid ClientId
        {
            get;
        }
    }
}
