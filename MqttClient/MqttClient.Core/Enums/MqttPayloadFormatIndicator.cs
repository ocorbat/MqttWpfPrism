using MqttClient.Core.Resources;
using MqttHelpers.Attributes;

namespace MqttClient.Core.Enums
{
    public enum MqttPayloadFormatIndicator
    {
        [LocalizedDescription("MqttPayloadFormatIndicator.Unspecified", typeof(EnumResources))]
        Unspecified = 0,
        [LocalizedDescription("MqttPayloadFormatIndicator.CharacterData", typeof(EnumResources))]
        CharacterData = 1
    }
}
