using MQTTnet.Packets;
using System.Collections;

namespace MqttServer.Backend.Core.Model
{
    public class ClientSubscriptionItem
    {
        public ClientSubscriptionItem(string id, MqttTopicFilter topicFilter, IDictionary sessionItems)
        {
            Id = id;
            TopicFilter = topicFilter;
            SessionItems = sessionItems;
        }

        public string Id { get; }

        public MqttTopicFilter TopicFilter { get; }

        public IDictionary SessionItems { get; }
    }
}
