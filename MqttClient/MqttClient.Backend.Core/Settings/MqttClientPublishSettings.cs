using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace MqttClient.Backend.Core.Settings
{
    public class MqttClientPublishSettings
    {
        public MqttClientPublishSettings() { }

        public string Topic { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public bool IsRetainOn { get; set; } = false;

        public MqttQualityOfServiceLevel QoS { get; set; } = MqttQualityOfServiceLevel.AtMostOnce;

        public MqttPayloadFormatIndicator PayloadFormatIndicator { get; set; } = MqttPayloadFormatIndicator.Unspecified;

        public string ResponseTopic { get; set; } = default!;

        public byte[] CorrelationData { get; set; } = default!;

        public MqttUserProperty UserProperty { get; set; } = new MqttUserProperty("Name", "Value");

        public uint MessageExpiryInterval { get; set; } = 86400;
    }
}

