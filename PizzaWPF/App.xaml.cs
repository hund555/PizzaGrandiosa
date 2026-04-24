using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PizzaWPF.Services;
using PizzaWPF.ViewModels;

namespace PizzaWPF
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient<IApiClient, ApiClient>(client =>
                    {
                        var apiBase = context.Configuration.GetSection("Api")["BaseUrl"];
                        client.BaseAddress = new Uri(apiBase ?? "https://localhost:5001/");
                    });

                    services.AddSingleton<IRabbitMqListener, RabbitMqListener>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddLogging(configure => configure.AddConsole());
                })
                .Build();

            _host.Start();

            // Start RabbitMQ listener after DI is configured
            var listener = _host.Services.GetRequiredService<IRabbitMqListener>();
            listener.StartListeningAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
                _host.Dispose();
            }

            base.OnExit(e);
        }
    }
}