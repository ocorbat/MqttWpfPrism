using MqttClient.Backend.Core;
using MqttClient.Backend.Core.Settings;
using MqttClient.Core.ViewModels;
using MqttClient.Modules.ModulePublisher.Enums;
using MqttCommon;
using MQTTnet.Protocol;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MqttClient.Modules.ModulePublisher.ViewModels
{
    public class PublisherViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private string messageText = "Enter Message";
        private bool isRetainModeOn = true;
        private string currentTopic = "Topic1";
        private string responseTopic = "response/Topic1";
        private MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce;
        private bool isCleanSessionOn = true;
        private bool isEnabled;
        private bool isExpanded = false;
        private uint messageExpiryInterval = 86400;
        private ContentTypeEnum currentContentType = Enums.ContentTypeEnum.PlainText;


        public PublisherViewModel()
        {
            PublishCommand = new DelegateCommand(PublishCommandExecute, PublishCommandCanExecute);
            DeleteCurrentTopicCommand = new DelegateCommand(DeleteCurrentTopicCommandExecute, DeleteCurrentTopicCommandCanExecute);
            DeleteResponseTopicCommand = new DelegateCommand(DeleteResponseTopicCommandExecute, DeleteResponseTopicCommandCanExecute);
        }

        private bool DeleteResponseTopicCommandCanExecute()
        {
            return true;
        }

        private void DeleteResponseTopicCommandExecute()
        {
            ResponseTopic = string.Empty;
        }

        private bool DeleteCurrentTopicCommandCanExecute()
        {
            return true;
        }

        private void DeleteCurrentTopicCommandExecute()
        {
            CurrentTopic = string.Empty;
        }

        public DelegateCommand PublishCommand { get; private set; }

        public DelegateCommand DeleteCurrentTopicCommand { get; private set; }

        public DelegateCommand DeleteResponseTopicCommand { get; private set; }

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
                    MqttClientController.ApplicationMessageReceived += MqttClientController_ApplicationMessageReceived;
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
            MqttClientPublishSettings settings = default!;
            string resourcePath;
            byte[] bytes = default!;

            settings = new MqttClientPublishSettings()
            {
                Topic = CurrentTopic,
                ResponseTopic = ResponseTopic,
                IsRetainOn = IsRetainModeOn,
                QoS = QualityOfServiceLevel,
                PayloadFormatIndicator = MqttPayloadFormatIndicator.Unspecified,
                MessageExpiryInterval = MessageExpiryInterval
            };

            switch (CurrentContentType)
            {
                case ContentTypeEnum.PlainText:
                    settings.ContentType = MimeTypes.TextPlain;
                    await MqttClientController.PublishAsync(MessageText, settings);
                    break;
                case ContentTypeEnum.ImageJpeg:
                    resourcePath = "pack://application:,,,/MqttResources;component/Resources/th.jpeg";
                    bytes = await GetBytesAsync(resourcePath);
                    settings.ContentType = MimeTypes.ImageJpeg;
                    await MqttClientController.PublishAsync(bytes, settings);
                    break;
                case ContentTypeEnum.ImagePng:
                    resourcePath = "pack://application:,,,/MqttResources;component/Resources/icon_det_256.png";
                    bytes = await GetBytesAsync(resourcePath);
                    settings.ContentType = MimeTypes.ImagePng;
                    await MqttClientController.PublishAsync(bytes, settings);
                    break;
            }

            IsExpanded = false;
        }

        private static async Task<byte[]> GetBytesAsync(string resourcePath)
        {
            var streamResourceInfo = Application.GetResourceStream(new System.Uri(resourcePath));
            if (streamResourceInfo == null)
            {
                throw new Exception("Resource not found: " + resourcePath);
            }

            using var memoryStream = new MemoryStream();
            await streamResourceInfo.Stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private bool PublishCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.PublishCommandCanExecute();
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
            IsEnabled = PublishCommandCanExecute();
            IsExpanded = false;
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
            IsEnabled = PublishCommandCanExecute();
            IsExpanded = false;
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            PublishCommand.RaiseCanExecuteChanged();
            IsEnabled = PublishCommandCanExecute();
            IsExpanded = false;
        }

        private async void MqttClientController_ApplicationMessageReceived(object sender, Backend.Events.ApplicationMessageReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ResponseTopic))
            {

                var settings = new MqttClientPublishSettings()
                {
                    Topic = e.ResponseTopic,
                    ContentType = MimeTypes.TextPlain,
                    IsRetainOn = IsRetainModeOn,
                    QoS = QualityOfServiceLevel,
                    PayloadFormatIndicator = MqttPayloadFormatIndicator.Unspecified,
                    MessageExpiryInterval = MessageExpiryInterval,
                    CorrelationData = e.CorrelationData
                };

                settings.ContentType = MimeTypes.TextPlain;
                await MqttClientController.PublishAsync("Message transmitted successfully", settings);
            }
        }




        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }



        public string ResponseTopic { get => responseTopic; set => SetProperty(ref responseTopic, value); }


        public ContentTypeEnum CurrentContentType { get => currentContentType; set => SetProperty(ref currentContentType, value); }



        public uint MessageExpiryInterval { get => messageExpiryInterval; set => SetProperty(ref messageExpiryInterval, value); }
    }
}
