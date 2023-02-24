using Mqtt.Backend.Common.Enums;

namespace Mqtt.Backend.Common.Extensions
{
    public static class MqttProtocolVersionExtensions
    {
        public static MqttProtocolVersion ToMqttProtocolVersion(this MQTTnet.Formatter.MqttProtocolVersion mqttProtocolVersion)
        {
            switch (mqttProtocolVersion)
            {
                case MQTTnet.Formatter.MqttProtocolVersion.Unknown:
                default:
                    return MqttProtocolVersion.Unknown;
                case MQTTnet.Formatter.MqttProtocolVersion.V310:
                    return MqttProtocolVersion.V310;
                case MQTTnet.Formatter.MqttProtocolVersion.V311:
                    return MqttProtocolVersion.V311;
                case MQTTnet.Formatter.MqttProtocolVersion.V500:
                    return MqttProtocolVersion.V500;


            }
        }
    }
}
