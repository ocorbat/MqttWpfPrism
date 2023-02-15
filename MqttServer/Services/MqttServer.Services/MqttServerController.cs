using MQTTnet;
using MqttServer.Services.Interfaces;
using System;

namespace MqttServer.Services
{
    public class MqttServerController : IMqttServerController
    {
        public MqttFactory MqttFactory { get; } = new MqttFactory();
        public MQTTnet.Server.MqttServer MqttServer { get; }

        public Guid ClientId { get; }


        public MqttServerController()
        {

        }
    }
}
