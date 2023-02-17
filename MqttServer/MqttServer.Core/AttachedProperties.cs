using MqttServer.Backend.Core;
using System.Windows;

namespace MqttServer.Core
{
    public static class AttachedProperties
    {
        public static readonly DependencyProperty MqttServerControllerProperty = DependencyProperty.RegisterAttached("MqttServerController",
              typeof(IMqttServerController),
              typeof(AttachedProperties),
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static IMqttServerController GetMqttServerController(DependencyObject element)
        {
            return (IMqttServerController)element?.GetValue(MqttServerControllerProperty);
        }

        public static void SetMqttServerController(DependencyObject element, IMqttServerController mqttServerController)
        {
            element?.SetValue(MqttServerControllerProperty, mqttServerController);
        }
    }
}
