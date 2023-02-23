



using MQTTnet.Formatter;

namespace MqttCore.Extensions
{
    public static class MqttProtocolVersionExtensions
    {
        public static MqttProtocolVersion ToMqttProtocolVersion(this Enums.MqttProtocolVersion protocolVersion)
        {
            return protocolVersion switch
            {
                Enums.MqttProtocolVersion.V310 => MqttProtocolVersion.V310,
                Enums.MqttProtocolVersion.V311 => MqttProtocolVersion.V311,
                Enums.MqttProtocolVersion.V500 => MqttProtocolVersion.V500,
                _ => MqttProtocolVersion.Unknown,
            };
        }
    }
}
