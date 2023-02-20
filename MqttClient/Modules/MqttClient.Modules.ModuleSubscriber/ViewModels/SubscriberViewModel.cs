using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;

namespace MqttClient.Modules.ModuleSubscriber.ViewModels
{
    public class SubscriberViewModel : BindableBase, IClientViewModelBase
    {
        public SubscriberViewModel()
        {
            SubscribeCommand = new DelegateCommand(SubscribeCommandExecute, SubscribeCommandCanExecute);
            UnsubscribeCommand = new DelegateCommand(UnsubscribeCommandExecute, UnsubscribeCommandCanExecute);
        }

        public DelegateCommand SubscribeCommand { get; set; }
        public DelegateCommand UnsubscribeCommand { get; set; }



        private async void SubscribeCommandExecute()
        {
            await MqttClientController.SubscribeAsync(CurrentTopic, QualityOfServiceLevel, IsNoLocalOn, isRetainAsPublishedOn, retainHandling);
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
            await MqttClientController.UnsubscribeAsync(CurrentTopic);
        }



        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
        }

        private MqttRetainHandling retainHandling = MqttRetainHandling.SendAtSubscribe;
        public MqttRetainHandling RetainHandling
        {
            get => retainHandling;
            set => SetProperty(ref retainHandling, value);
        }


        private string currentTopic = "Topic1";
        public string CurrentTopic
        {
            get => currentTopic;
            set => SetProperty(ref currentTopic, value);
        }




        private IMqttClientController mqttClientController;

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    SubscribeCommand.RaiseCanExecuteChanged();
                    UnsubscribeCommand.RaiseCanExecuteChanged();
                    IsEnabled = SubscribeCommandCanExecute();
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                }
            }
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            IsEnabled = SubscribeCommandCanExecute();
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            IsEnabled = SubscribeCommandCanExecute();
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
            IsEnabled = SubscribeCommandCanExecute();
        }

        private bool isNoLocalOn = true;

        public bool IsNoLocalOn
        {
            get => isNoLocalOn;
            set => SetProperty(ref isNoLocalOn, value);
        }

        private bool isRetainAsPublishedOn = false;

        public bool IsRetainAsPublishedOn
        {
            get => isRetainAsPublishedOn;
            set => SetProperty(ref isRetainAsPublishedOn, value);
        }




        private bool isEnabled;

        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }
    }
}
