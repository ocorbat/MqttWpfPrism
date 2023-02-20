using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MqttClient.Modules.ModuleMessageHistory.ViewModels
{
    public class MessageHistoryViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public MessageHistoryViewModel()
        {
            Message = "View A from your Prism Module";
            ClearCommand = new DelegateCommand(ClearCommandExecute, ClearCommandCanExecute);
        }



        private void ClearCommandExecute()
        {
            ListReceivedMessages.Clear();
        }

        private bool ClearCommandCanExecute()
        {
            return ListReceivedMessages.Any();
        }

        public DelegateCommand ClearCommand { get; set; }

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    ClearCommand.RaiseCanExecuteChanged();
                    MqttClientController.ApplicationMessageReceived += MqttClientController_ApplicationMessageReceived;
                    //ConnectCommand.RaiseCanExecuteChanged();
                    //DisconnectCommand.RaiseCanExecuteChanged();
                    //MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    //MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    //MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                    //MqttClientController.ApplicationMessageReceived += MqttClientController_ApplicationMessageReceived;
                }
            }
        }

        private void MqttClientController_ApplicationMessageReceived(object sender, Backend.Events.ApplicationMessageReceivedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                ListReceivedMessages.Add(e.ApplicationMessage.ToString());
            });
            ClearCommand.RaiseCanExecuteChanged();
        }

        private ObservableCollection<string> listReceivedMessages = new ObservableCollection<string>();

        public ObservableCollection<string> ListReceivedMessages { get => listReceivedMessages; set => SetProperty(ref listReceivedMessages, value); }
    }
}
