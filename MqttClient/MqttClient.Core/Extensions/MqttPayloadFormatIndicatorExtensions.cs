using MQTTnet.Protocol;

namespace MqttClient.Core.Extensions
{
    public static class MqttPayloadFormatIndicatorExtensions
    {
        public static MqttPayloadFormatIndicator ToMqttPayloadFormatIndicator(this Enums.MqttPayloadFormatIndicator payloadFormatIndicator)
        {
            return payloadFormatIndicator switch
            {
                Enums.MqttPayloadFormatIndicator.CharacterData => MqttPayloadFormatIndicator.CharacterData,
                _ => MqttPayloadFormatIndicator.Unspecified,
            };
        }
    }
}
