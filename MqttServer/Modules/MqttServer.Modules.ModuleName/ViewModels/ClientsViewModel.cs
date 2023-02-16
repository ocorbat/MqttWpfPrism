using MQTTnet.Server;
using MqttServer.Core.ViewModels;
using MqttServer.Services.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MqttServer.Modules.ModuleName.ViewModels
{
    public class ClientsViewModel : BindableBase, IServerViewModelBase, INavigationAware, IConfirmNavigationRequest
    {
        private IEnumerable<MqttClientStatus> connectedClients;
        private ObservableCollection<MqttClientStatus> subscribedClients;
        private ICollectionView subscribedClientsView;

        public ClientsViewModel(IRegionManager regionManager, IMessageService messageService)
        {


            GetConnectedClientsCommand = new DelegateCommand(GetConnectedClientsCommandExecute, GetConnectedClientsCommandCanExecute);

            subscribedClients = new ObservableCollection<MqttClientStatus>();
            SubscribedClientsView = CollectionViewSource.GetDefaultView(subscribedClients);
        }

        public DelegateCommand GetConnectedClientsCommand { get; set; }



        public IEnumerable<MqttClientStatus> ConnectedClients
        {
            get => connectedClients;
            set => SetProperty(ref connectedClients, value);
        }

        public ICollectionView SubscribedClientsView
        {
            get => subscribedClientsView;
            set => SetProperty(ref subscribedClientsView, value);
        }

        private bool GetConnectedClientsCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.MqttServer != null && MqttServerController.MqttServer.IsStarted;
        }

        private async void GetConnectedClientsCommandExecute()
        {
            ConnectedClients = MqttServerController?.MqttServer != null ? await MqttServerController.MqttServer.GetClientsAsync() : (IEnumerable<MqttClientStatus>)null;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {

        }

        private IMqttServerController mqttServerController;

        public IMqttServerController MqttServerController
        {
            get => mqttServerController;
            set
            {
                if (SetProperty(ref mqttServerController, value))
                {
                    GetConnectedClientsCommand.RaiseCanExecuteChanged();
                    MqttServerController.ServerStarted += MqttServerController_ServerStarted;
                    MqttServerController.ServerStopped += MqttServerController_ServerStopped;
                }
            }
        }

        private void MqttServerController_ServerStopped(object sender, EventArgs e)
        {
            GetConnectedClientsCommand.RaiseCanExecuteChanged();
        }

        private void MqttServerController_ServerStarted(object sender, EventArgs e)
        {
            GetConnectedClientsCommand.RaiseCanExecuteChanged();
        }
    }
}
