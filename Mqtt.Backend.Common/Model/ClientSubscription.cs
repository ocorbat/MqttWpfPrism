using MQTTnet.Client;
using MQTTnet.Packets;

namespace Mqtt.Backend.Common.Model
{
    public class ClientSubscription
    {
        public ClientSubscription(MqttClientSubscribeResultCode resultCode, MqttTopicFilter topicFilter)
        {
            ResultCode = resultCode;
            TopicFilter = topicFilter;
        }

        public MqttClientSubscribeResultCode ResultCode { get; set; }

        public MqttTopicFilter TopicFilter { get; set; }
    }
}
