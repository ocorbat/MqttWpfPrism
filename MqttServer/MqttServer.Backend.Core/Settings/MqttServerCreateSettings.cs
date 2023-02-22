namespace MqttServer.Backend.Core.Settings
{
    public class MqttServerCreateSettings
    {
        public MqttServerCreateSettings() { }

        public int PortNumber { get; set; } = MqttCommon.Constants.PortNumber;

        public bool IsPersistentSessions { get; set; } = true;
    }
}
