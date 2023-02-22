using MQTTnet.Formatter;

namespace MqttClient.Backend.Core
{
    public class MqttClientConnectSettings
    {
        public MqttClientConnectSettings() { }


        public int PortNumber { get; set; } = 1883;

        public bool IsCleanSession { get; set; } = true;

        public MqttProtocolVersion ProtocolVersion { get; set; } = MqttProtocolVersion.V500;

        public TimeSpan KeepAlivePeriod { get; set; } = new TimeSpan(0, 1, 0);

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}


