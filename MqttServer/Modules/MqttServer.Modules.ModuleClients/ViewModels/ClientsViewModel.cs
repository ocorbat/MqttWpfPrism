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

namespace MqttServer.Modules.ModuleClients.ViewModels
{
    public class ClientsViewModel : BindableBase, IMqttServerControllerViewModel, INavigationAware, IConfirmNavigationRequest
    {
        private IMqttServerController mqttServerController;
        private IEnumerable<MqttClientStatus> connectedClients = new List<MqttClientStatus>();
        private readonly ObservableCollection<ConnectedClientViewModel> subscribedClients;
        private ICollectionView subscribedClientsView;

        public ClientsViewModel(IRegionManager regionManager, IMessageService messageService)
        {
            subscribedClients = new ObservableCollection<ConnectedClientViewModel>();
            SubscribedClientsView = CollectionViewSource.GetDefaultView(subscribedClients);
            GetConnectedClientsCommand = new DelegateCommand(GetConnectedClientsCommandExecute, GetConnectedClientsCommandCanExecute);
        }

        public DelegateCommand GetConnectedClientsCommand { get; set; }

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

        private bool GetConnectedClientsCommandCanExecute()
        {
            return MqttServerController != null && MqttServerController.GetConnectedClientsCommandCanExecute();
        }

        private async void GetConnectedClientsCommandExecute()
        {
            ConnectedClients = await MqttServerController.RefreshConnectedClientsAsync();
        }

        private async void MqttServerController_ClientSubscribedTopic(object sender, Backend.Events.ClientSubscribedTopicEventArgs e)
        {
            var connectedClientViewModel = new ConnectedClientViewModel
            {
                ClientId = e.ClientId,
                Topic = e.Topic
            };

            await Application.Current.Dispatcher.BeginInvoke(() =>
            {
                subscribedClients.Add(connectedClientViewModel);
            });
        }

        private void MqttServerController_ClientUnsubscribedTopic(object sender, Backend.Events.ClientUnsubscribedTopicEventArgs e)
        {
            var id = e.ClientId;
            var toto = subscribedClients.Where(c => c.ClientId == id);

            foreach (var item in toto)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    subscribedClients.Remove(item);
                });
            }
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
