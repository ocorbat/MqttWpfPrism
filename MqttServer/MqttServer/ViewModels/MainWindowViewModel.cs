using Prism.Mvvm;

namespace MqttServer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "MQTT Server";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
