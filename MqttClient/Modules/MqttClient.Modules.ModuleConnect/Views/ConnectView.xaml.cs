using MqttClient.Modules.ModuleConnect.ViewModels;
using MqttClient.Services.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace MqttClient.Modules.ModuleConnect.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class ConnectView : UserControl
    {
        public ConnectView()
        {
            InitializeComponent();
        }

        static ConnectView()
        {
            MqttClientControllerProperty = DependencyProperty.RegisterAttached(
                "MqttClientController",
                typeof(IMqttClientController),
                typeof(ConnectView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
                    MqttClientControllerPropertyChangedCallback));
        }

        public static readonly DependencyProperty MqttClientControllerProperty;

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
            if (d is not ConnectView view)
            {
                return;
            }

            var toto = (IMqttClientController)e.NewValue;

            var dataContext = (ConnectViewModel)view.DataContext;
            dataContext.MqttClientController = (IMqttClientController)e.NewValue;
        }
    }
}
