using MqttClient.Services.Interfaces;

namespace MqttClient.Core.ViewModels
{
    public interface IClientViewModelBase
    {
        IMqttClientController MqttClientController { get; set; }
    }
}
