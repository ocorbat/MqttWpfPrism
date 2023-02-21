﻿using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MqttCommon;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Resources;

namespace MqttClient.Modules.ModulePublisher.ViewModels
{
    public class PublisherViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private string messageText = "Enter Message";
        private bool isRetainModeOn = true;
        private string currentTopic = "Topic1";
        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce;
        private bool isCleanSessionOn = true;
        private bool isEnabled;

        public PublisherViewModel()
        {
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
            PublishImageCommand = new DelegateCommand(PublishImageCommandExecute, PublishImageCommandCanExecute);
        }
        public DelegateCommand PublishCommand { get; private set; }
        public DelegateCommand PublishImageCommand { get; private set; }

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    PublishCommand.RaiseCanExecuteChanged();
                    IsEnabled = PublishCommandCanExecute();
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
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

        public string MessageText
        {
            get => messageText;
            set => SetProperty(ref messageText, value);
        }

        private async void PublishCommandExecute()
        {
            await MqttClientController.PublishAsync(CurrentTopic, MessageText, IsRetainModeOn, QualityOfServiceLevel);
        }

        private bool PublishCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.PublishCommandCanExecute();
        }

        private async void PublishImageCommandExecute()
        {
            //string resourcePath = "pack://application:,,,/MqttResources;component/Resources/icon_det_256.png";
            string resourcePath = "pack://application:,,,/MqttResources;component/Resources/th.jpeg";
            StreamResourceInfo streamResourceInfo = Application.GetResourceStream(new System.Uri(resourcePath));
            if (streamResourceInfo == null)
            {
                throw new Exception("Resource not found: " + resourcePath);
            }

            var memoryStream = new System.IO.MemoryStream();
            await streamResourceInfo.Stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();

            await MqttClientController.PublishAsync(CurrentTopic, bytes, MimeTypes.ImageJpeg, IsRetainModeOn, QualityOfServiceLevel);
        }

        private bool PublishImageCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.PublishCommandCanExecute();
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
            PublishImageCommand.RaiseCanExecuteChanged();
            IsEnabled = PublishCommandCanExecute();
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
            PublishImageCommand.RaiseCanExecuteChanged();
            IsEnabled = PublishCommandCanExecute();
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
            PublishImageCommand.RaiseCanExecuteChanged();
            IsEnabled = PublishCommandCanExecute();
        }
    }
}
