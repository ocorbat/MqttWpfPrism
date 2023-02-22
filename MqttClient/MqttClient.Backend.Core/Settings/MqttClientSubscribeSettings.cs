using MQTTnet.Protocol;

namespace MqttClient.Backend.Core.Settings
{
    public class MqttClientSubscribeSettings
    {
        public MqttClientSubscribeSettings() { }

        public string Topic { get; set; } = default!;

        public MqttQualityOfServiceLevel QoS { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

        public bool NoLocalOn { get; set; } = false;

        public bool RetainAsPublishedOn { get; set; } = false;
        public MqttRetainHandling RetainHandling { get; set; } = MqttRetainHandling.SendAtSubscribe;
    }
}



