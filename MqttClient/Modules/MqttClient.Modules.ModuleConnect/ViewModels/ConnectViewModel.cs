using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MQTTnet.Formatter;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;

namespace MqttClient.Modules.ModuleConnect.ViewModels
{
    public class ConnectViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private string username = "admin";
        private string password = "1234";
        private string status = string.Empty;
        private bool isCleanSessionOn = true;
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
            await MqttClientController.ConnectAsync(IsCleanSessionOn, ProtocolVersion, Username, Password);
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

        private void MqttClientController_ApplicationMessageReceived(object sender, Backend.Events.ApplicationMessageReceivedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ListReceivedMessages.Add(ReceivedMessage);
            });

            ReceivedMessage = e.ApplicationMessage.ToString();
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            Status = $"Client {MqttClientController.ClientId} is disconnected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            Status = $"Client {MqttClientController.ClientId} is connected";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            Status = $"Client {MqttClientController.ClientId} is connecting";
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }

        private ObservableCollection<string> listReceivedMessages = new ObservableCollection<string>();

        public ObservableCollection<string> ListReceivedMessages { get => listReceivedMessages; set => SetProperty(ref listReceivedMessages, value); }
    }
}
