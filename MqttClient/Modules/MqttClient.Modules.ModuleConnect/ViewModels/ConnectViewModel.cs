using MqttClient.Backend.Core;
using MqttClient.Backend.Core.Settings;
using MqttClient.Core.ViewModels;
using MqttCommon;
using MqttCore.Enums;
using MqttCore.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;

namespace MqttClient.Modules.ModuleConnect.ViewModels
{
    public class ConnectViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private string username = "admin";
        private string password = "1234";
        private string status = string.Empty;
        private bool isCleanSessionOn = false;
        private int keepAlivePeriod = 60;
        private bool isExpanded = false;
        private int portNumber = Constants.Port5004;
        private uint sessionExpiryInterval = 86400;
        private MqttProtocolVersion protocolVersion = MqttProtocolVersion.V500;

        public ConnectViewModel()
        {
            ConnectCommand = new DelegateCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            DisconnectCommand = new DelegateCommand(DisonnectCommandExecute, DisonnectCommandCanExecute);
        }

        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public bool IsCleanSessionOn
        {
            get => isCleanSessionOn;
            set => SetProperty(ref isCleanSessionOn, value);
        }

        public MqttProtocolVersion ProtocolVersion
        {
            get => protocolVersion;
            set => SetProperty(ref protocolVersion, value);
        }

        private async void ConnectCommandExecute()
        {
            var settings = new MqttClientConnectSettings()
            {
                PortNumber = PortNumber,
                IsCleanSession = IsCleanSessionOn,
                ProtocolVersion = ProtocolVersion.ToMqttProtocolVersion(),
                KeepAlivePeriod = TimeSpan.FromSeconds(KeepAlivePeriod),
                Username = Username,
                Password = Password,
                SessionExpiryInterval = SessionExpiryInterval
            };

            await MqttClientController.ConnectAsync(settings);
            IsExpanded = false;
        }

        private bool ConnectCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.ConnectCommandCanExecute();
        }

        private async void DisonnectCommandExecute()
        {
            await MqttClientController.DisconnectAsync();
        }

        private bool DisonnectCommandCanExecute()
        {
            return MqttClientController != null && MqttClientController.DisonnectCommandCanExecute();
        }





        private string receivedMessage;
        public string ReceivedMessage
        {
            get => receivedMessage;
            set => SetProperty(ref receivedMessage, value);
        }



        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    ConnectCommand.RaiseCanExecuteChanged();
                    DisconnectCommand.RaiseCanExecuteChanged();
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                    MqttClientController.ApplicationMessageReceived += MqttClientController_ApplicationMessageReceived;
                }
            }
        }

        private void MqttClientController_ApplicationMessageReceived(object sender, Backend.Events.MqttApplicationMessageReceivedEventArgs e)
        {

            switch (e.ContentType)
            {
                case MimeTypes.ImagePng:
                case MimeTypes.ImageJpeg:
                    MemoryStream memoryStream = new(e.Payload);
                    //Bitmap image = new(memoryStream);
                    //string filePath = Guid.NewGuid().ToString() + ".png";
                    //image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    ReceivedImage = e.Payload;
                    break;
                case MimeTypes.TextPlain:
                    var payloadString = Convert.ToString(e.Payload);

                    // Convert Payload to string
                    var payload = e.Payload == null ? null : System.Text.Encoding.UTF8.GetString(e.Payload);

                    //if (payload != null)
                    //{
                    //    listReceivedData.Add(payload);
                    //    OnApplicationMessageReceived(new Events.ApplicationMessageReceivedEventArgs(payload));
                    //}

                    ReceivedMessage = payload;
                    break;
                default:
                    ReceivedMessage = string.Empty;
                    ReceivedImage = null;
                    break;

            }
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            Status = $"Client {MqttClientController.ClientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            IsExpanded = false;
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            Status = $"Client {MqttClientController.ClientId} is connected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            IsExpanded = false;
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            Status = $"Client {MqttClientController.ClientId} is connecting";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            IsExpanded = false;
        }



        public int PortNumber { get => portNumber; set => SetProperty(ref portNumber, value); }

        private byte[] receivedImage;

        public byte[] ReceivedImage { get => receivedImage; set => SetProperty(ref receivedImage, value); }



        public int KeepAlivePeriod { get => keepAlivePeriod; set => SetProperty(ref keepAlivePeriod, value); }



        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }



        public uint SessionExpiryInterval { get => sessionExpiryInterval; set => SetProperty(ref sessionExpiryInterval, value); }


    }
}
