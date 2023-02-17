
using MqttClient.Backend.Core;
using Prism.Mvvm;

namespace MqttClient.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowDataContext
    {
        private string _title = "MQTT Client";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(IMqttClientController mqttClientController)
        {
            MqttClientController = mqttClientController;
        }

        private IMqttClientController mqttClientController;

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set => SetProperty(ref mqttClientController, value);
        }
    }
}
