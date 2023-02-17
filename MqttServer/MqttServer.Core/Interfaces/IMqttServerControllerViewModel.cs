using MqttServer.Backend.Core;

namespace MqttServer.Core.Interfaces
{
    public interface IMqttServerControllerViewModel
    {
        IMqttServerController MqttServerController { get; set; }
    }
}
