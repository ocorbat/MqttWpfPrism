using Mqtt.Backend.Common.Events;
using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using Prism.Mvvm;

namespace MqttClient.Modules.ModuleMessage.ViewModels
{
    public class MessageViewModel : BindableBase, IClientViewModelBase
    {
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public MessageViewModel()
        {
            Message = "View A from your Prism Module";
        }

        private IMqttClientController mqttClientController;

        public IMqttClientController MqttClientController
        {
            get => mqttClientController;
            set
            {
                if (SetProperty(ref mqttClientController, value))
                {
                    MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;

                    MqttClientController.OutputMessage += MqttClientController_OutputMessage;
                }
            }
        }

        private void MqttClientController_OutputMessage(object sender, OutputMessageEventArgs e)
        {
            Message = e.Message;
        }

        private void MqttClientController_ClientConnecting(object sender, Backend.Events.MqttClientConnectingEventArgs e)
        {
            Message = e.Message;
        }

        private void MqttClientController_ClientDisconnected(object sender, Backend.Events.MqttClientDisconnectedEventArgs e)
        {
            Message = e.Message;
        }

        private void MqttClientController_ClientConnected(object sender, Backend.Events.MqttClientConnectedEventArgs e)
        {
            Message = e.Message;
        }
    }
}
