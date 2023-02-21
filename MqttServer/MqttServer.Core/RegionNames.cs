namespace MqttServer.Core
{
    public static class RegionNames
    {
        public static string ClientsRegion { get; } = nameof(ClientsRegion);
        public static string ExecuteRegion { get; } = nameof(ExecuteRegion);
        public static string MessageRegion { get; } = nameof(MessageRegion);
        public static string PublisherRegion { get; } = nameof(PublisherRegion);
    }
}
