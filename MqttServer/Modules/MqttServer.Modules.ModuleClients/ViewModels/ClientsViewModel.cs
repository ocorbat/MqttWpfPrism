using MQTTnet.Server;
using MqttServer.Backend.Core;
using MqttServer.Backend.Services.Interfaces;
using MqttServer.Core.Interfaces;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MqttServer.Modules.ModuleName.ViewModels
{
    public class ClientsViewModel : BindableBase, IMqttServerControllerViewModel, INavigationAware, IConfirmNavigationRequest
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
            return MqttServerController == null ? false : MqttServerController.GetConnectedClientsCommandCanExecute();
        }

        private async void GetConnectedClientsCommandExecute()
        {
            ConnectedClients = await MqttServerController.RefreshConnectedClientsAsync();
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
                    MqttServerController.ClientConnected += MqttServerController_ClientConnected;
                    MqttServerController.ClientDisconnected += MqttServerController_ClientDisconnected;
                    MqttServerController.ClientSubscribedTopic += MqttServerController_ClientSubscribedTopic;
                    MqttServerController.ClientUnsubscribedTopic += MqttServerController_ClientUnsubscribedTopic;
                }
            }
        }

        private async void MqttServerController_ClientSubscribedTopic(object sender, Backend.Events.ClientSubscribedTopicEventArgs e)
        {
            var id = e.ClientId;
            var connectedClients = await MqttServerController.MqttServer.GetClientsAsync();
            var toto = connectedClients.SingleOrDefault(c => c.Id == id);

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!subscribedClients.Any(c => c.Id == id))
                {
                    subscribedClients.Add(toto);
                }
            });
        }

        private void MqttServerController_ClientUnsubscribedTopic(object sender, Backend.Events.ClientUnsubscribedTopicEventArgs e)
        {
            var id = e.ClientId;
            var toto = subscribedClients.SingleOrDefault(c => c.Id == id);

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                subscribedClients.Remove(toto);
            });
        }

        private void MqttServerController_ClientDisconnected(object sender, Backend.Events.ClientDisconnectedEventArgs e)
        {
            ConnectedClients = e.CurrentConnectedClients;
        }

        private void MqttServerController_ClientConnected(object sender, Backend.Events.ClientConnectedEventArgs e)
        {
            ConnectedClients = e.CurrentConnectedClients;
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
