using System.Windows;

namespace MqttClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //public static readonly DependencyProperty MqttClientControllerProperty = DependencyProperty.RegisterAttached("MqttClientController",
        //       typeof(IMqttClientController),
        //       typeof(MainWindow),
        //       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        //public static IMqttClientController GetMqttClientController(DependencyObject element)
        //{
        //    return (IMqttClientController)element?.GetValue(MqttClientControllerProperty);
        //}

        //public static void SetMqttClientController(DependencyObject element, IMqttClientController mqttClientController)
        //{
        //    element?.SetValue(MqttClientControllerProperty, mqttClientController);
        //}

    }
}
