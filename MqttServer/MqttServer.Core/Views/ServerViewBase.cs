using MqttServer.Core.ViewModels;
using MqttServer.Services.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace MqttServer.Core.Views
{
    public abstract class ServerViewBase : UserControl
    {
        public static readonly DependencyProperty MqttServerControllerProperty;

        static ServerViewBase()
        {
            MqttServerControllerProperty = DependencyProperty.RegisterAttached(
                "MqttServerController",
                typeof(IMqttServerController),
                typeof(ServerViewBase),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
                    MqttServerControllerPropertyChangedCallback));
        }

        public static IMqttServerController GetMqttServerController(DependencyObject element)
        {
            return (IMqttServerController)element?.GetValue(MqttServerControllerProperty);
        }

        public static void SetMqttServerController(DependencyObject element,
            IMqttServerController controller)
        {
            element?.SetValue(MqttServerControllerProperty, controller);
        }

        private static void MqttServerControllerPropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ServerViewBase view)
            {
                return;
            }

            var dataContext = (IServerViewModelBase)view.DataContext;
            dataContext.MqttServerController = (IMqttServerController)e.NewValue;
        }
    }
}
