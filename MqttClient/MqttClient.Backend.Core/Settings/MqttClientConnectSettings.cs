using MQTTnet.Formatter;

namespace MqttClient.Backend.Core.Settings
{
    public class MqttClientConnectSettings
    {
        public MqttClientConnectSettings() { }

        public int PortNumber { get; set; } = Mqtt.Backend.Common.Constants.PortNumber;

        public bool IsCleanSession { get; set; } = true;

        public MqttProtocolVersion ProtocolVersion { get; set; } = MqttProtocolVersion.V500;

        public TimeSpan KeepAlivePeriod { get; set; } = new TimeSpan(0, 1, 0);

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

        public uint SessionExpiryInterval { get; set; } = 86400;

        public bool RequestResponseInformation { get; set; } = true;

        public bool RequestProblemInformation { get; set; } = true;
    }
}


