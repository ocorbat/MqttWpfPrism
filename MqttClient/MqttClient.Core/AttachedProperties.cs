using MqttClient.Services.Interfaces;
using System.Windows;

namespace MqttClient.Core
{
    public static class AttachedProperties
    {
        public static readonly DependencyProperty MqttClientControllerProperty = DependencyProperty.RegisterAttached("MqttClientController",
               typeof(IMqttClientController),
               typeof(AttachedProperties),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static IMqttClientController GetMqttClientController(DependencyObject element)
        {
            return (IMqttClientController)element?.GetValue(MqttClientControllerProperty);
        }

        public static void SetMqttClientController(DependencyObject element, IMqttClientController mqttClientController)
        {
            element?.SetValue(MqttClientControllerProperty, mqttClientController);
        }
    }
}
