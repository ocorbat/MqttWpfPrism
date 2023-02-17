using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;

namespace MqttClient.Modules.ModulePublisher.ViewModels
{
    public class PublisherViewModel : BindableBase, IClientViewModelBase
    {
        private string sendMessageText = "Enter Message";
        private bool isRetainModeOn = false;
        private string currentTopic = "Topic1";
        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
        private bool isCleanSessionOn = true;

        public PublisherViewModel()
        {
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
        }
        public DelegateCommand PublishCommand { get; set; }

        private async void PublishCommandExecute()
        {
            await MqttClientController.PublishAsync(CurrentTopic, SendMessageText, IsRetainModeOn, QualityOfServiceLevel);
        }

        private bool PublishCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.PublishCommandCanExecute();
        }

        public bool IsRetainModeOn
        {
            get => isRetainModeOn;
            set => SetProperty(ref isRetainModeOn, value);
        }


        public string CurrentTopic
        {
            get => currentTopic;
            set => SetProperty(ref currentTopic, value);
        }


        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
        }


        public bool IsCleanSessionOn
        {
            get => isCleanSessionOn;
            set => SetProperty(ref isCleanSessionOn, value);
        }

        public string SendMessageText
        {
            get => sendMessageText;
            set => SetProperty(ref sendMessageText, value);
        }


        private IMqttClientController mqttClientController;

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    PublishCommand.RaiseCanExecuteChanged();
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                }
            }
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
        }
    }
}
