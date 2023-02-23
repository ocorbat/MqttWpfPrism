using MqttCore.Resources;
using MqttHelpers.Attributes;

namespace MqttCore.Enums
{
    public enum MqttProtocolVersion
    {
        [LocalizedDescription("MqttProtocolVersion.Unknown", typeof(EnumResources))]
        Unknown = 0,
        [LocalizedDescription("MqttProtocolVersion.V310", typeof(EnumResources))]
        V310 = 3,
        [LocalizedDescription("MqttProtocolVersion.V311", typeof(EnumResources))]
        V311 = 4,
        [LocalizedDescription("MqttProtocolVersion.V500", typeof(EnumResources))]
        V500 = 5
    }
}
