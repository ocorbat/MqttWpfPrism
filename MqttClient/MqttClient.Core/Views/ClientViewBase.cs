using MqttClient.Backend.Core;
using MqttClient.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MqttClient.Core.Views
{
    public abstract class ClientViewBase : UserControl
    {
        public static readonly DependencyProperty MqttClientControllerProperty;

        static ClientViewBase()
        {
            MqttClientControllerProperty = DependencyProperty.RegisterAttached(
                "MqttClientController",
                typeof(IMqttClientController),
                typeof(ClientViewBase),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
                    MqttClientControllerPropertyChangedCallback));
        }

        public static IMqttClientController GetMqttClientController(DependencyObject element)
        {
            return (IMqttClientController)element?.GetValue(MqttClientControllerProperty);
        }

        public static void SetMqttClientController(DependencyObject element,
            IMqttClientController controller)
        {
            element?.SetValue(MqttClientControllerProperty, controller);
        }

        private static void MqttClientControllerPropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ClientViewBase view)
            {
                return;
            }

            var dataContext = (IClientViewModelBase)view.DataContext;
            dataContext.MqttClientController = (IMqttClientController)e.NewValue;
        }
    }
}
