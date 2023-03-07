using Prism.Mvvm;

namespace MqttServer.Modules.ModuleClients.ViewModels
{
    internal class ClientSubscriptionItemViewModel : BindableBase
    {
        public ClientSubscriptionItemViewModel() { }

        private string clientId;
        public string ClientId
        {
            get => clientId;
            set => SetProperty(ref clientId, value);
        }

        private string topic;
        public string Topic
        {
            get => topic;
            set => SetProperty(ref topic, value);
        }
    }
}
