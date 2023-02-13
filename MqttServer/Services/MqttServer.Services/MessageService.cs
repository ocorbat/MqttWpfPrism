using MqttServer.Services.Interfaces;

namespace MqttServer.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
