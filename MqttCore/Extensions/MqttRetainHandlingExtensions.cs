﻿

using MQTTnet.Protocol;

namespace MqttCore.Extensions
{
    public static class MqttRetainHandlingExtensions
    {
        public static MqttRetainHandling ToMqttRetainHandling(this Enums.MqttRetainHandling retainHandling)
        {
            return retainHandling switch
            {
                Enums.MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly => MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly,
                Enums.MqttRetainHandling.DoNotSendOnSubscribe => MqttRetainHandling.DoNotSendOnSubscribe,
                _ => MqttRetainHandling.SendAtSubscribe,
            };
        }
    }
}
