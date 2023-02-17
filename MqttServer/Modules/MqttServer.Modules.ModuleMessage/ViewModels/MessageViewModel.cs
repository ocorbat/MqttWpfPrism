using MqttServer.Backend.Core;
using MqttServer.Core.Interfaces;
using Prism.Mvvm;

namespace MqttServer.Modules.ModuleMessage.ViewModels
{
    public class MessageViewModel : BindableBase, IMqttServerControllerViewModel
    {
        private string _message;


        public MessageViewModel()
        {
            Message = "View A from your Prism Module";
        }


        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        private IMqttServerController mqttServerController;

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
            Message = e.Message;
        }

        private void MqttServerController_ServerStopped(object sender, System.EventArgs e)
        {
            Message = "Server stopped";
        }

        private void MqttServerController_ServerStarted(object sender, System.EventArgs e)
        {
            Message = "Server started";
        }


    }
}
