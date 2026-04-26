using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PizzaWPF.ViewModels;

namespace PizzaWPF.Services
{
    // Minimal safe listener: simulates incoming orders if no real RabbitMQ integration is enabled.
    // Extend this to connect to RabbitMQ.Client when you want real messaging.
    public class RabbitMqListener : IRabbitMqListener, IDisposable
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly IConfiguration _config;
        private readonly MainWindowViewModel _vm;
        private CancellationTokenSource? _cts;
        private Task? _background;

        public RabbitMqListener(ILogger<RabbitMqListener> logger, IConfiguration config, MainWindowViewModel vm)
        {
            _logger = logger;
            _config = config;
            _vm = vm;
        }

        public Task StartListeningAsync()
        {
            _cts = new CancellationTokenSource();
            _background = Task.Run(() => RunAsync(_cts.Token), CancellationToken.None);
            _logger.LogInformation("RabbitMqListener started (simulated).");
            return Task.CompletedTask;
        }

        private async Task RunAsync(CancellationToken token)
        {
            // If you later add real RabbitMQ connection logic, replace this loop.
            var counter = 1;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Simulate receiving an order id
                    var orderId = $"SIM-{DateTime.UtcNow:HHmmss}-{counter++}";
                    _logger.LogInformation("Simulated incoming order {OrderId}", orderId);

                    // Enqueue on the UI view model
                    _vm.EnqueueOrder(orderId);

                    await Task.Delay(TimeSpan.FromSeconds(5), token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in simulated listener loop");
                    await Task.Delay(TimeSpan.FromSeconds(5), token).ConfigureAwait(false);
                }
            }
        }

        public Task StopAsync()
        {
            try
            {
                _cts?.Cancel();
                if (_background != null)
                {
                    return _background.ContinueWith(_ => { }, TaskScheduler.Default);
                }
            }
            catch { }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}