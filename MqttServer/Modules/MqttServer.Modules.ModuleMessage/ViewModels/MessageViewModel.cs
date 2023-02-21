using MqttServer.Backend.Core;
using MqttServer.Core.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace MqttServer.Modules.ModuleMessage.ViewModels
{
    public class MessageViewModel : BindableBase, IMqttServerControllerViewModel
    {
        private IMqttServerController mqttServerController;
        private ObservableCollection<MessageItemViewModel> messages = new();

        public MessageViewModel()
        {

        }

        public ObservableCollection<MessageItemViewModel> Messages
        {
            get => messages;
            set => SetProperty(ref messages, value);
        }

        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    //MqttServerController.ClientConnecting += MqttClientController_ClientConnecting;
                    //MqttServerController.ClientConnected += MqttClientController_ClientConnected;
                    //MqttServerController.ClientDisconnected += MqttClientController_ClientDisconnected;

                    MqttServerController.OutputMessage += MqttServerController_OutputMessage;

                    MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                }
            }
        }

        private void MqttServerController_OutputMessage(object sender, MqttCommon.Events.OutputMessageEventArgs e)
        {
            Messages.Insert(0, new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = e.Message });
        }

        private void MqttServerController_ServerStopped(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Messages.Insert(0, new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = "Server stopped" });
            });
        }

        private void MqttServerController_ServerStarted(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Messages.Insert(0, new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = "Server started" });
            });
        }
    }
}
