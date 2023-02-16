using MqttServer.Core.Interfaces;
using Prism.Mvvm;

namespace MqttServer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "MQTT Server";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(IMqttServerController mqttServerController)
        {
            MqttServerController = mqttServerController;
        }

        private IMqttServerController mqttClientController;

        public IMqttServerController MqttServerController
        {
            get => mqttClientController;
            set => SetProperty(ref mqttClientController, value);
        }
    }
}
