namespace MqttClient.Backend.Core
{
    public class MqttClientUnsubscribeSettings
    {
        public MqttClientUnsubscribeSettings() { }

        public string Topic { get; set; } = default!;
    }
}
