using MQTTnet.Protocol;
using MqttServer.Backend.Core;
using MqttServer.Core.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace MqttServer.Modules.ModulePublisher.ViewModels
{
    public class PublisherViewModel : BindableBase, IMqttServerControllerViewModel
    {
        private IMqttServerController mqttServerController;
        private string sendMessageText = "Enter Message";
        private bool isRetainModeOn = false;
        private string currentTopic = "Topic1";
        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;

        public PublisherViewModel()
        {
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
        }
        public DelegateCommand PublishCommand { get; set; }


        private async void PublishCommandExecute()
        {
            await MqttServerController.PublishAsync(CurrentTopic, SendMessageText, IsRetainModeOn, QualityOfServiceLevel);
        }

        private bool PublishCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.PublishCommandCanExecute();
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

        public string SendMessageText
        {
            get => sendMessageText;
            set => SetProperty(ref sendMessageText, value);
        }

        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
        }

        private bool isEnabled;

        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }


        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    PublishCommand.RaiseCanExecuteChanged();
                    IsEnabled = PublishCommandCanExecute();

                    MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                    //StartServerCommand.RaiseCanExecuteChanged();
                    //StopServerCommand.RaiseCanExecuteChanged();

                    //MqttServerController.ClientConnected += MqttServerController_ClientConnected;
                    //MqttServerController.ClientDisconnected += MqttServerController_ClientDisconnected;

                    //MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    //MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                }
            }
        }

        private void MqttServerController_ServerStopped(object sender, System.EventArgs e)
        {
            IsEnabled = false;
            PublishCommand.RaiseCanExecuteChanged();
        }

        private void MqttServerController_ServerStarted(object sender, System.EventArgs e)
        {
            IsEnabled = true;
            PublishCommand.RaiseCanExecuteChanged();
        }
    }
}
