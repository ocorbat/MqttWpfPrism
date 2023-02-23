using MqttCore.Resources;
using MqttHelpers.Attributes;

namespace MqttCore.Enums
{
    public enum MqttPayloadFormatIndicator
    {
        [LocalizedDescription("MqttPayloadFormatIndicator.Unspecified", typeof(EnumResources))]
        Unspecified = 0,
        [LocalizedDescription("MqttPayloadFormatIndicator.CharacterData", typeof(EnumResources))]
        CharacterData = 1
    }
}
