using MQTTnet;
using MqttServer.Services.Interfaces;
using System;

namespace MqttServer.Services
{
    public class MqttServerController : IMqttServerController
    {
        public MqttFactory MqttFactory { get; } = new MqttFactory();
        public MQTTnet.Server.MqttServer MqttServer { get; set; }

        public Guid ClientId { get; }


        public MqttServerController()
        {

        }

        public event EventHandler<EventArgs> ServerStarted;
        public event EventHandler<EventArgs> ServerStopped;

        public void OnServerStarted(EventArgs e)
        {
            ServerStarted?.Invoke(this, e);
        }

        public void OnServerStopped(EventArgs e)
        {
            ServerStopped?.Invoke(this, e);
        }

    }
}
