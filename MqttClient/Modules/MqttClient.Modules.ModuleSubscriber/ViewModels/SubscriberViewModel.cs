using MqttClient.Core.ViewModels;
using MqttClient.Services.Interfaces;
using MqttCommon.Extensions;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading;

namespace MqttClient.Modules.ModuleSubscriber.ViewModels
{
    public class SubscriberViewModel : BindableBase, IClientViewModelBase
    {
        private string receivedMessage;

        public SubscriberViewModel()
        {
            SubscribeCommand = new DelegateCommand(SubscribeCommandExecute, SubscribeCommandCanExecute);
            UnsubscribeCommand = new DelegateCommand(UnsubscribeCommandExecute, UnsubscribeCommandCanExecute);
        }

        public DelegateCommand SubscribeCommand { get; set; }
        public DelegateCommand UnsubscribeCommand { get; set; }



        private async void SubscribeCommandExecute()
        {
            MqttClientSubscribeOptions mqttSubscribeOptions;

            switch (QualityOfServiceLevel)
            {
                case MqttQualityOfServiceLevel.AtMostOnce:
                default:
                    mqttSubscribeOptions = MqttClientController.MqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(CurrentTopic).WithAtMostOnceQoS();
                   })
               .Build();
                    break;
                case MqttQualityOfServiceLevel.AtLeastOnce:
                    mqttSubscribeOptions = MqttClientController.MqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(CurrentTopic).WithAtLeastOnceQoS();
                   })
                    .Build();

                    break;
                case MqttQualityOfServiceLevel.ExactlyOnce:
                    mqttSubscribeOptions = MqttClientController.MqttFactory.CreateSubscribeOptionsBuilder()
               .WithTopicFilter(
                   f =>
                   {
                       f.WithTopic(CurrentTopic).WithExactlyOnceQoS();
                   })
               .Build();
                    break;
            }

            try
            {
                MqttClientSubscribeResult response;

                using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    response = await MqttClientController.MqttClient.SubscribeAsync(mqttSubscribeOptions, timeoutToken.Token);
                }

                Debug.WriteLine($"MQTT client {MqttClientController.MqttClient.Options.ClientId} subscribed to topic '{CurrentTopic}'.");
                // The response contains additional data sent by the server after subscribing.
                MqttClientController.OnOutputMessage(new Events.OutputMessageEventArgs(response.DumpToString()));
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

        private bool SubscribeCommandCanExecute()
        {
            return MqttClientController == null
                ? false
                : MqttClientController.MqttClient == null ? false : MqttClientController.MqttClient.IsConnected;
        }



        private bool UnsubscribeCommandCanExecute()
        {
            if (MqttClientController == null)
            {
                return false;
            }
            return MqttClientController.MqttClient == null ? false : MqttClientController.MqttClient.IsConnected;
        }

        private async void UnsubscribeCommandExecute()
        {
            MqttClientUnsubscribeResult result;
            var mqttUnsubscribeOptions = MqttClientController.MqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(CurrentTopic)
                .Build();

            result = await MqttClientController.MqttClient.UnsubscribeAsync(mqttUnsubscribeOptions);
            MqttClientController.OnOutputMessage(new Events.OutputMessageEventArgs(result.DumpToString()));
        }



        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
        public MqttQualityOfServiceLevel QualityOfServiceLevel
        {
            get => qualityOfServiceLevel;
            set => SetProperty(ref qualityOfServiceLevel, value);
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
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                }
            }
        }

        private void MqttClientController_ClientConnecting(object sender, MqttClientConnectingEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            SubscribeCommand.RaiseCanExecuteChanged();
            UnsubscribeCommand.RaiseCanExecuteChanged();
        }
    }
}
