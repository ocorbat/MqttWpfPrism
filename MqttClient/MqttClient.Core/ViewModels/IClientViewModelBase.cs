using MqttClient.Backend.Core;

namespace MqttClient.Core.ViewModels
{
    public interface IClientViewModelBase
    {
        IMqttClientController MqttClientController { get; set; }
    }
}
