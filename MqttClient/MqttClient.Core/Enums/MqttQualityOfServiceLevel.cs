using MqttClient.Core.Resources;
using MqttHelpers.Attributes;

namespace MqttClient.Core.Enums
{
    public enum MqttQualityOfServiceLevel
    {
        [LocalizedDescription("MqttQualityOfServiceLevel.AtMostOnce", typeof(EnumResources))]
        AtMostOnce = 0x00,
        [LocalizedDescription("MqttQualityOfServiceLevel.AtLeastOnce", typeof(EnumResources))]
        AtLeastOnce = 0x01,
        [LocalizedDescription("MqttQualityOfServiceLevel.ExactlyOnce", typeof(EnumResources))]
        ExactlyOnce = 0x02
    }
}
