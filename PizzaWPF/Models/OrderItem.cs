using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PizzaWPF.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        private string _orderId = string.Empty;
        public string OrderId
        {
            get => _orderId;
            set => SetProperty(ref _orderId, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}