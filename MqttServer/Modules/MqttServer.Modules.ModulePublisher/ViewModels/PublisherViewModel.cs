using Mqtt.Backend.Common;
using MqttCore.Enums;
using MqttCore.Extensions;
using MqttServer.Backend.Core;
using MqttServer.Backend.Core.Settings;
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
        private ContentTypeEnum currentContentType = ContentTypeEnum.PlainText;

        public PublisherViewModel()
        {
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
            DeleteRetainedMessagesCommand = new DelegateCommand(DeleteRetainedMessagesCommandExecute, DeleteRetainedMessagesCommandCanExecute);
            DeleteCurrentTopicCommand = new DelegateCommand(DeleteCurrentTopicCommandExecute, DeleteCurrentTopicCommandCanExecute);
        }
        public DelegateCommand PublishCommand { get; set; }

        public DelegateCommand DeleteRetainedMessagesCommand { get; set; }

        public DelegateCommand DeleteCurrentTopicCommand { get; private set; }

        private bool DeleteCurrentTopicCommandCanExecute()
        {
            return true;
        }

        private void DeleteCurrentTopicCommandExecute()
        {
            CurrentTopic = string.Empty;
        }

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
            var settings = new MqttServerPublishSettings()
            {
                Topic = CurrentTopic,
                ContentType = MimeTypes.TextPlain,
                IsRetainOn = IsRetainModeOn,
                QoS = QualityOfServiceLevel.ToMqttQualityOfServiceLevel(),
                PayloadFormatIndicator = MqttPayloadFormatIndicator.Unspecified.ToMqttPayloadFormatIndicator()
            };

            await MqttServerController.PublishAsync(SendMessageText, settings);
            IsExpanded = false;
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
            IsExpanded = false;
        }

        private void MqttServerController_ServerStarted(object sender, System.EventArgs e)
        {
            IsEnabled = true;
            PublishCommand.RaiseCanExecuteChanged();
            DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
            IsExpanded = false;
        }

        private bool isExpanded = false;

        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }

        public ContentTypeEnum CurrentContentType { get => currentContentType; set => SetProperty(ref currentContentType, value); }
    }
}
