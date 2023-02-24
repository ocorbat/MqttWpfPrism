using Mqtt.Backend.Common.Enums;
using Mqtt.Backend.Common.Extensions;

namespace Mqtt.Backend.Common
{
    public class MqttClientStatus
    {
        public MqttClientStatus(MQTTnet.Server.MqttClientStatus mqttClientStatus)
        {
            Id = mqttClientStatus.Id;
            Endpoint = mqttClientStatus.Endpoint;
            ProtocolVersion = mqttClientStatus.ProtocolVersion.ToMqttProtocolVersion();
            ConnectedTimestamp = mqttClientStatus.ConnectedTimestamp;
            LastPacketReceivedTimestamp = mqttClientStatus.LastPacketReceivedTimestamp;
            LastPacketSentTimestamp = mqttClientStatus.LastPacketSentTimestamp;
            LastNonKeepAlivePacketReceivedTimestamp = mqttClientStatus.LastNonKeepAlivePacketReceivedTimestamp;
            ReceivedApplicationMessagesCount = mqttClientStatus.ReceivedApplicationMessagesCount;
            SentApplicationMessagesCount = mqttClientStatus.SentApplicationMessagesCount;
            ReceivedPacketsCount = mqttClientStatus.ReceivedPacketsCount;
            SentPacketsCount = mqttClientStatus.SentPacketsCount;
            BytesSent = mqttClientStatus.BytesSent;
            BytesReceived = mqttClientStatus.BytesReceived;
        }


        public string Id { get; set; }

        public string Endpoint { get; set; }

        public MqttProtocolVersion ProtocolVersion { get; set; }

        public DateTime ConnectedTimestamp { get; set; }

        public DateTime LastPacketReceivedTimestamp { get; set; }

        public DateTime LastPacketSentTimestamp { get; set; }

        public DateTime LastNonKeepAlivePacketReceivedTimestamp { get; set; }

        public long ReceivedApplicationMessagesCount { get; set; }

        public long SentApplicationMessagesCount { get; set; }

        public long ReceivedPacketsCount { get; set; }

        public long SentPacketsCount { get; set; }

        public long BytesSent { get; set; }

        public long BytesReceived { get; set; }
    }
}
