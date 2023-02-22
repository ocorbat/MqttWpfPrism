using MqttClient.Backend.Core;

namespace MqttClient.ViewModels
{
    internal interface IMainWindowDataContext
    {
        string Title { get; set; }

        IMqttClientController MqttClientController { get; }
    }
}
