using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PizzaWPF.Commands
{
    // Simple, generic async command for WPF.
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Predicate<T?>? _canExecute;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged;

        public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_isExecuting) return false;
            if (_canExecute is null) return true;
            return _canExecute((T?)parameter);
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync((T?)parameter).ConfigureAwait(false);
        }

        public async Task ExecuteAsync(T? parameter)
        {
            if (!CanExecute(parameter)) return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute(parameter).ConfigureAwait(false);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}