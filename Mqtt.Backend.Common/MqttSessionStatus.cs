namespace Mqtt.Backend.Common
{
    public class MqttSessionStatus
    {
        public MqttSessionStatus(MQTTnet.Server.MqttSessionStatus mqttSessionStatus)
        {
            Id = mqttSessionStatus.Id;
            PendingApplicationMessagesCount = mqttSessionStatus.PendingApplicationMessagesCount;
            CreatedTimestamp = mqttSessionStatus.CreatedTimestamp;
        }

        public string Id { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public long PendingApplicationMessagesCount { get; set; }
    }
}
