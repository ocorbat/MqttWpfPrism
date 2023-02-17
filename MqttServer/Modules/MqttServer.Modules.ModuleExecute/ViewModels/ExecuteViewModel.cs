using MqttServer.Backend.Core;
using MqttServer.Core.Dispose;
using MqttServer.Core.Interfaces;
using Prism.Commands;
using System.Windows;

namespace MqttServer.Modules.ModuleExecute.ViewModels
{
    public class ExecuteViewModel : DisposableBindableBase, IMqttServerControllerViewModel
    {
        // To detect redundant calls
        private bool _disposedValue;

        ~ExecuteViewModel() => Dispose(false);

        private IMqttServerController mqttServerController;
        private string status = string.Empty;

        public ExecuteViewModel()
        {
            StartServerCommand = new DelegateCommand(StartServerCommandExecute, StartServerCommandCanExecute);
            StopServerCommand = new DelegateCommand(StopServerCommandExecute, StopServerCommandCanExecute);
        }

        public DelegateCommand StartServerCommand { get; set; }
        public DelegateCommand StopServerCommand { get; set; }

        private bool StartServerCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.StartServerCommandCanExecute();
        }

        private async void StartServerCommandExecute()
        {
            var mqttServer = MqttServerController?.CreateServer();

            if (mqttServer != null)
            {
                await MqttServerController.StartAsync();
            }
        }

        private bool StopServerCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.StopServerCommandCanExecute();
        }

        private async void StopServerCommandExecute()
        {
            await MqttServerController?.StopAsync();
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }






        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                _disposedValue = true;
            }

            // Call the base class implementation.
            base.Dispose(disposing);
        }

        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    StartServerCommand.RaiseCanExecuteChanged();
                    StopServerCommand.RaiseCanExecuteChanged();

                    MqttServerController.ClientConnected += MqttServerController_ClientConnected;
                    MqttServerController.ClientDisconnected += MqttServerController_ClientDisconnected;

                    MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                }
            }
        }

        private void MqttServerController_ServerStopped(object sender, System.EventArgs e)
        {
            Status = "Server stopped";
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
        }

        private void MqttServerController_ServerStarted(object sender, System.EventArgs e)
        {
            Status = "Server started";
            StartServerCommand.RaiseCanExecuteChanged();
            StopServerCommand.RaiseCanExecuteChanged();
        }

        private void MqttServerController_ClientDisconnected(object sender, Backend.Events.ClientDisconnectedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
        }

        private void MqttServerController_ClientConnected(object sender, Backend.Events.ClientConnectedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                //textBlock_ServerConnected.Text = arg.ClientId.ToString();
            });
        }
    }
}
