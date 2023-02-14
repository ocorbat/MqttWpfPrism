using Prism.Mvvm;

namespace MqttClient.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "MQTT Client";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel()
        {

        }
    }
}
