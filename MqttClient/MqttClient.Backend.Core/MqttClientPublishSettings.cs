using MQTTnet.Protocol;

namespace MqttClient.Backend.Core
{
    public class MqttClientPublishSettings
    {
        public MqttClientPublishSettings() { }

        public string Topic { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public bool IsRetainOn { get; set; } = false;

        public MqttQualityOfServiceLevel QoS { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

        public MqttPayloadFormatIndicator PayloadFormatIndicator { get; set; } = MqttPayloadFormatIndicator.Unspecified;
    }
}

