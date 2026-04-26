using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PizzaWPF.Services;
using PizzaWPF.ViewModels;
using Microsoft.Extensions.Http;
using System.Net.Http;

namespace PizzaWPF
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Normalize configured API base so relative paths combine correctly
                    var apiBase = context.Configuration.GetSection("Api")["BaseUrl"] ?? "https://localhost:5001/";
                    if (!apiBase.EndsWith("/")) apiBase += "/";

                    services.AddHttpClient<IApiClient, ApiClient>(client =>
                    {
                        client.BaseAddress = new Uri(apiBase);
                    });

                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<IRabbitMqListener, RabbitMqListener>();
                    services.AddSingleton<MainWindow>();
                    services.AddLogging(configure => configure.AddConsole());
                })
                .Build();

            try
            {
                // Start host and listener in an awaited manner so startup failures are observable
                await _host.StartAsync();

                var listener = _host.Services.GetRequiredService<IRabbitMqListener>();
                await listener.StartListeningAsync();

                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Log and show a friendly message then exit
                var logger = _host.Services.GetService<ILogger<App>>();
                logger?.LogError(ex, "Application failed to start");
                MessageBox.Show("Application failed to start. Check logs for details.", "Startup error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
            {
                try
                {
                    // Stop the RabbitMQ listener if it exposes StopAsync
                    var listener = _host.Services.GetService<IRabbitMqListener>();
                    if (listener != null)
                        await listener.StopAsync();

                    await _host.StopAsync(TimeSpan.FromSeconds(5));
                }
                catch { }
                finally
                {
                    _host.Dispose();
                }
            }

            base.OnExit(e);
        }
    }
}