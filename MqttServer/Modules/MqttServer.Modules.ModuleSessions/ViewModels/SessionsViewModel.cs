using MQTTnet.Server;
using MqttServer.Backend.Core;
using MqttServer.Core.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;

namespace MqttServer.Modules.ModuleSessions.ViewModels
{
    public class SessionsViewModel : BindableBase, IMqttServerControllerViewModel
    {
        private IMqttServerController mqttServerController;


        public SessionsViewModel()
        {
            GetSessionsCommand = new DelegateCommand(GetSessionsCommandExecute, GetSessionsCommandCanExecute);
        }

        private bool GetSessionsCommandCanExecute()
        {
            return true;
        }

        private async void GetSessionsCommandExecute()
        {
            CurrentSessions = await MqttServerController.GetSessionsAsync();
        }

        public DelegateCommand GetSessionsCommand { get; set; }

        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    GetSessionsCommand.RaiseCanExecuteChanged();
                    //DeleteRetainedMessagesCommand.RaiseCanExecuteChanged();
                    //IsEnabled = PublishCommandCanExecute();

                    //MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    //MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                    //StartServerCommand.RaiseCanExecuteChanged();
                    //StopServerCommand.RaiseCanExecuteChanged();

                    //MqttServerController.ClientConnected += MqttServerController_ClientConnected;
                    //MqttServerController.ClientDisconnected += MqttServerController_ClientDisconnected;

                    //MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    //MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                }
            }
        }

        private IEnumerable<MqttSessionStatus> currentSessions = new List<MqttSessionStatus>();
        public IEnumerable<MqttSessionStatus> CurrentSessions
        {
            get => currentSessions;
            set => SetProperty(ref currentSessions, value);
        }


        private bool isEnabled = true;

        public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }
    }
}
