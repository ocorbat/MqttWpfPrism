namespace MqttServer.Modules.ModuleClients.Model
{
    internal class ConnectedClient
    {
        public ConnectedClient() { }

        public string ClientId { get; set; }

        public string Topic { get; set; }
    }
}
