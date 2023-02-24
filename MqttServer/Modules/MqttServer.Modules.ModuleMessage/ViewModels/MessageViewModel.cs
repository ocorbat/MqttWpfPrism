using Mqtt.Backend.Common.Events;
using MqttServer.Backend.Core;
using MqttServer.Core.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace MqttServer.Modules.ModuleMessage.ViewModels
{
    public class MessageViewModel : BindableBase, IMqttServerControllerViewModel
    {
        private IMqttServerController mqttServerController;
        private readonly ObservableCollection<MessageItemViewModel> messages = new();
        private ICollectionView messagesView;




        public MessageViewModel()
        {
            MessagesView = CollectionViewSource.GetDefaultView(messages);
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


        public ICollectionView MessagesView
        {
            get => messagesView;
            set => SetProperty(ref messagesView, value);
        }

        private void MqttServerController_OutputMessage(object sender, OutputMessageEventArgs e)
        {
            messages.Add(new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = e.Message });
        }

        private void MqttServerController_ServerStopped(object sender, EventArgs e)
        {
            var currentThread = Thread.CurrentThread;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                messages.Add(new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = "Server stopped" });
            }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);

            //Application.Current.Dispatcher.Invoke(() => { messages.Add(new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = "Server stopped" }); });
        }

        private void MqttServerController_ServerStarted(object sender, EventArgs e)
        {
            var currentThread = Thread.CurrentThread;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                messages.Add(new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = "Server started" });
            }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);

            //Application.Current.Dispatcher.Invoke(() => { messages.Add(new MessageItemViewModel() { Timestamp = DateTime.UtcNow, Message = "Server started" }); });
        }
    }
}
