using MqttServer.Backend.Services.Interfaces;

namespace MqttServer.Backend.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
