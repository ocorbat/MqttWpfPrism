using MqttClient.Services.Interfaces;

namespace MqttClient.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
