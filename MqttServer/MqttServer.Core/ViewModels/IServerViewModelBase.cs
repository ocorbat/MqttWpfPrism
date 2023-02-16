using MqttServer.Services.Interfaces;

namespace MqttServer.Core.ViewModels
{
    public interface IServerViewModelBase
    {
        IMqttServerController MqttServerController { get; set; }
    }
}
