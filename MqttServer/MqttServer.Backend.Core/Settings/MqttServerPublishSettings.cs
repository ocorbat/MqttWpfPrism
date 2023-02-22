using MQTTnet.Protocol;

namespace MqttServer.Backend.Core.Settings
{
    public class MqttServerPublishSettings
    {
        public MqttServerPublishSettings() { }

        public string Topic { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public bool IsRetainOn { get; set; } = false;

        public MqttQualityOfServiceLevel QoS { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

        public MqttPayloadFormatIndicator PayloadFormatIndicator { get; set; } = MqttPayloadFormatIndicator.Unspecified;
    }
}
