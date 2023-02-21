using Prism.Mvvm;
using System;

namespace MqttServer.Modules.ModuleMessage.ViewModels
{
    public class MessageItemViewModel : BindableBase
    {
        private DateTime timestamp;
        public DateTime Timestamp
        {
            get => timestamp;
            set => SetProperty(ref timestamp, value);
        }

        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }
    }
}
