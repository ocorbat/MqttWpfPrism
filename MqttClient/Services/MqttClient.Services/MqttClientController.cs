using MqttClient.Services.Interfaces;
using MQTTnet.Client;
using System;

namespace MqttClient.Services
{
    public class MqttClientController : IMqttClientController
    {
        public IMqttClient MqttClient { get; set; }
        public Guid ClientId { get; private set; }

        public MqttClientController()
        {
            ClientId = Guid.NewGuid();
        }

    }
}
