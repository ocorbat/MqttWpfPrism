using MQTTnet.Protocol;

namespace MqttClient.Core.Extensions
{
    public static class MqttQualityOfServiceLevelExtensions
    {
        public static MqttQualityOfServiceLevel ToMqttQualityOfServiceLevel(this Enums.MqttQualityOfServiceLevel qualityOfServiceLevel)
        {
            return qualityOfServiceLevel switch
            {
                Enums.MqttQualityOfServiceLevel.AtMostOnce => MqttQualityOfServiceLevel.AtMostOnce,
                Enums.MqttQualityOfServiceLevel.AtLeastOnce => MqttQualityOfServiceLevel.AtLeastOnce,
                Enums.MqttQualityOfServiceLevel.ExactlyOnce => MqttQualityOfServiceLevel.ExactlyOnce,
                _ => MqttQualityOfServiceLevel.AtMostOnce,
            };
        }
    }
}
