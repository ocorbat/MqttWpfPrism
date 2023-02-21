using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using MqttCommon;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MqttClient.Modules.ModuleMessageHistory.ViewModels
{
    public class MessageHistoryViewModel : BindableBase, IClientViewModelBase
    {
        private IMqttClientController mqttClientController;
        private ObservableCollection<string> listReceivedMessages = new();


        public MessageHistoryViewModel()
        {
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
                    //MqttClientController.ClientConnecting += MqttClientController_ClientConnecting;
                    //MqttClientController.ClientConnected += MqttClientController_ClientConnected;
                    //MqttClientController.ClientDisconnected += MqttClientController_ClientDisconnected;
                }
            }
        }

        private void MqttClientController_ApplicationMessageReceived(object sender, Backend.Events.ApplicationMessageReceivedEventArgs e)
        {
            switch (e.ContentType)
            {
                case MimeTypes.ImagePng:
                case MimeTypes.ImageJpeg:
                    //MemoryStream memoryStream = new(e.ApplicationMessage);
                    //Bitmap image = new(memoryStream);
                    //string filePath = Guid.NewGuid().ToString() + ".png";
                    //image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    //ReceivedImage = e.ApplicationMessage;
                    break;
                case MimeTypes.TextPlain:
                    var payloadString = Convert.ToString(e.ApplicationMessage);

                    // Convert Payload to string
                    var payload = e.ApplicationMessage == null ? null : System.Text.Encoding.UTF8.GetString(e.ApplicationMessage);

                    //if (payload != null)
                    //{
                    //    listReceivedData.Add(payload);
                    //    OnApplicationMessageReceived(new Events.ApplicationMessageReceivedEventArgs(payload));
                    //}

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        ListReceivedMessages.Add(payload);
                    });
                    break;

                default:
                    break;

            }

            ClearCommand.RaiseCanExecuteChanged();
        }



        public ObservableCollection<string> ListReceivedMessages { get => listReceivedMessages; set => SetProperty(ref listReceivedMessages, value); }
    }
}
