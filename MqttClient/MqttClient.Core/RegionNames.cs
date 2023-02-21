namespace MqttClient.Core
{
    public static class RegionNames
    {
        public static string ConnectRegion { get; } = nameof(ConnectRegion);
        public static string MessageRegion { get; } = nameof(MessageRegion);
        public static string PublisherRegion { get; } = nameof(PublisherRegion);
        public static string SubscriberRegion { get; } = nameof(SubscriberRegion);
        public static string MessageHistoryRegion { get; } = nameof(MessageHistoryRegion);
    }
}
