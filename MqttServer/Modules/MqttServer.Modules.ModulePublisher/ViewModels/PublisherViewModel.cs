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
        private bool isRetainModeOn = true;
        private string currentTopic = "Topic1";
        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce;
        private bool isEnabled;

        public PublisherViewModel()
        {
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
            DeleteRetainedMessagesCommand = new DelegateCommand(DeleteRetainedMessagesCommandExecute, DeleteRetainedMessagesCommandCanExecute);
        }
        public DelegateCommand PublishCommand { get; set; }

        public DelegateCommand DeleteRetainedMessagesCommand { get; set; }

        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    PublishCommand.RaiseCanExecuteChanged();
                    DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
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

        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }

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

        private async void PublishCommandExecute()
        {
            await MqttServerController.PublishAsync(CurrentTopic, SendMessageText, IsRetainModeOn, QualityOfServiceLevel);
        }

        private bool PublishCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.PublishCommandCanExecute();
        }

        private async void DeleteRetainedMessagesCommandExecute()
        {
            await MqttServerController.DeleteRetainedMessagesAsync();
        }

        private bool DeleteRetainedMessagesCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.DeleteRetainedMessagesCommandCanExecute();
        }

        private void MqttServerController_ServerStopped(object sender, System.EventArgs e)
        {
            IsEnabled = false;
            PublishCommand.RaiseCanExecuteChanged();
            DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
        }

        private void MqttServerController_ServerStarted(object sender, System.EventArgs e)
        {
            IsEnabled = true;
            PublishCommand.RaiseCanExecuteChanged();
            DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
        }
    }
}
