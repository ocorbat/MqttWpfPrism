using MqttClient.Core.ViewModels;
using MqttClient.Services.Interfaces;
using MqttCommon.Extensions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading;

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
            var applicationMessage = new MqttApplicationMessageBuilder()
               .WithTopic(CurrentTopic)
               .WithPayload(SendMessageText)
               .WithRetainFlag(IsRetainModeOn)
               .WithQualityOfServiceLevel(QualityOfServiceLevel)
               .Build();

            try
            {
                MqttClientPublishResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                {
                    response = await MqttClientController.MqttClient.PublishAsync(applicationMessage, timeoutToken.Token);

                    if (response.IsSuccess)
                    {
                        MqttClientController.OnOutputMessage(new Events.OutputMessageEventArgs(response.DumpToString()));
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                MqttClientController.OnOutputMessage(new Events.OutputMessageEventArgs($"({e})"));
            }
            catch (MQTTnet.Exceptions.MqttCommunicationTimedOutException e)
            {
                MqttClientController.OnOutputMessage(new Events.OutputMessageEventArgs($"({e})"));
            }
        }

        private bool PublishCommandCanExecute()
        {
            return MqttClientController == null
                ? false
                : MqttClientController.MqttClient == null ? false : MqttClientController.MqttClient.IsConnected;
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

        private void MqttClientController_ClientConnecting(object sender, MqttClientConnectingEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
        }
    }
}
