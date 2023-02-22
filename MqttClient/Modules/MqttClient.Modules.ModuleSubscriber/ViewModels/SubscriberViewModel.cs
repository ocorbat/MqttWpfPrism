using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;

namespace MqttClient.Modules.ModuleSubscriber.ViewModels
{
    public class SubscriberViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
        private MqttRetainHandling retainHandling = MqttRetainHandling.SendAtSubscribe;
        private string currentTopic = "Topic1";
        private bool isNoLocalOn = true;
        private bool isRetainAsPublishedOn = false;
        private bool isEnabled;

        public SubscriberViewModel()
        {
            SubscribeCommand = new DelegateCommand(SubscribeCommandExecute, SubscribeCommandCanExecute);
            UnsubscribeCommand = new DelegateCommand(UnsubscribeCommandExecute, UnsubscribeCommandCanExecute);
            DeleteRetainedMessagesCommand = new DelegateCommand(DeleteRetainedMessagesCommandExecute, DeleteRetainedMessagesCommandCanExecute);
        }

        public DelegateCommand SubscribeCommand { get; set; }
        public DelegateCommand UnsubscribeCommand { get; set; }
        public DelegateCommand DeleteRetainedMessagesCommand { get; set; }
        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    SubscribeCommand.RaiseCanExecuteChanged();
                    UnsubscribeCommand.RaiseCanExecuteChanged();
                    DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
                    IsEnabled = SubscribeCommandCanExecute();
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                }
            }
        }

        public bool IsNoLocalOn
        {
            get => isNoLocalOn;
            set => SetProperty(ref isNoLocalOn, value);
        }

        public bool IsRetainAsPublishedOn
        {
            get => isRetainAsPublishedOn;
            set => SetProperty(ref isRetainAsPublishedOn, value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }

        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
        }

        public MqttRetainHandling RetainHandling
        {
            get => retainHandling;
            set => SetProperty(ref retainHandling, value);
        }

        public string CurrentTopic
        {
            get => currentTopic;
            set => SetProperty(ref currentTopic, value);
        }

        private async void SubscribeCommandExecute()
        {
            var settings = new MqttClientSubscribeSettings()
            {
                Topic = CurrentTopic,
                QoS = QualityOfServiceLevel,
                NoLocalOn = IsNoLocalOn,
                RetainAsPublishedOn = IsRetainAsPublishedOn,
                RetainHandling = RetainHandling
            };


            await MqttClientController.SubscribeAsync(settings);
        }

        private bool SubscribeCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.SubscribeCommandCanExecute();
        }

        private bool UnsubscribeCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.UnsubscribeCommandCanExecute();
        }

        private async void UnsubscribeCommandExecute()
        {
            var settings = new MqttClientUnsubscribeSettings()
            {
                Topic = CurrentTopic,
            };

            await MqttClientController.UnsubscribeAsync(settings);
        }

        private async void DeleteRetainedMessagesCommandExecute()
        {
            await MqttClientController.PublishEmptyAsync(CurrentTopic);
        }

        private bool DeleteRetainedMessagesCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.DeleteRetainedMessagesCommandCanExecute();
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
            IsEnabled = SubscribeCommandCanExecute();
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
            IsEnabled = SubscribeCommandCanExecute();
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
            IsEnabled = SubscribeCommandCanExecute();
        }
    }
}
