using MqttClient.Backend.Services.Interfaces;

namespace MqttClient.Backend.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
