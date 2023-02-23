using MqttClient.Core.Resources;
using MqttHelpers.Attributes;

namespace MqttClient.Core.Enums
{
    public enum MqttRetainHandling
    {
        [LocalizedDescription("MqttRetainHandling.SendAtSubscribe", typeof(EnumResources))]
        SendAtSubscribe = 0,
        [LocalizedDescription("MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly", typeof(EnumResources))]
        SendAtSubscribeIfNewSubscriptionOnly = 1,
        [LocalizedDescription("MqttRetainHandling.DoNotSendOnSubscribe", typeof(EnumResources))]
        DoNotSendOnSubscribe = 2
    }
}
