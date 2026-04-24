using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using PizzaWPF.Models;
using PizzaWPF.Services;

namespace PizzaWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly Dispatcher _dispatcher;

        public ObservableCollection<OrderItem> Orders { get; } = new();
        public AsyncRelayCommand<OrderItem> AcceptCommand { get; }
        public AsyncRelayCommand<OrderItem> DeclineCommand { get; }

        private string _statusBar = "Ready";
        public string StatusBar
        {
            get => _statusBar;
            set => SetProperty(ref _statusBar, value);
        }

        public MainWindowViewModel(IApiClient apiClient, ILogger<MainWindowViewModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
            _dispatcher = Dispatcher.CurrentDispatcher;

            AcceptCommand = new AsyncRelayCommand<OrderItem>(AcceptAsync);
            DeclineCommand = new AsyncRelayCommand<OrderItem>(DeclineAsync);
        }

        public void EnqueueOrder(string orderId)
        {
            // Called from RabbitMQ listener on background thread
            _dispatcher.Invoke(() =>
            {
                Orders.Add(new OrderItem { OrderId = orderId, StatusMessage = "Pending" });
                StatusBar = $"Received order {orderId}";
            });
        }

        private async Task AcceptAsync(OrderItem? item)
        {
            if (item is null) return;

            item.IsBusy = true;
            item.StatusMessage = "Accepting...";
            try
            {
                var ok = await _apiClient.SendAcceptAsync(item.OrderId);
                item.StatusMessage = ok ? "Accepted" : "Failed to accept";
                StatusBar = ok ? $"Order {item.OrderId} accepted" : $"Failed to accept {item.OrderId}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Accept failed for {OrderId}", item.OrderId);
                item.StatusMessage = "Accept error";
                StatusBar = $"Error sending accept for {item.OrderId}";
            }
            finally
            {
                item.IsBusy = false;
            }
        }

        private async Task DeclineAsync(OrderItem? item)
        {
            if (item is null) return;

            item.IsBusy = true;
            item.StatusMessage = "Declining...";
            try
            {
                var ok = await _apiClient.SendDeclineAsync(item.OrderId);
                item.StatusMessage = ok ? "Declined" : "Failed to decline";
                StatusBar = ok ? $"Order {item.OrderId} declined" : $"Failed to decline {item.OrderId}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decline failed for {OrderId}", item.OrderId);
                item.StatusMessage = "Decline error";
                StatusBar = $"Error sending decline for {item.OrderId}";
            }
            finally
            {
                item.IsBusy = false;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}