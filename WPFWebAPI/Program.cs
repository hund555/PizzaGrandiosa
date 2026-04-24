
using WPFWebAPI.Services;
using WPFWebAPI.Services.Service_Interfaces;

namespace WPFWebAPI
{
    using Endpoints;
    using WPFWebAPI.Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<IOrderService, OrderService>();
            builder.Services.AddSingleton<SseConnectionManager>();

            // Http client used to call back the main API when responding to orders
            builder.Services.AddHttpClient("Api", client => {
                var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";
                client.BaseAddress = new Uri(apiBase);
            });

            

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<RabbitMQConfigOption>(
                builder.Configuration.GetSection("RabbitmqConfig"));
            builder.Services.AddScoped<IOrderServiceRabbitMQ, OrderServiceRabbitMQ>();

            var app = builder.Build();

            var rabbit = app.Services.GetRequiredService<IOrderServiceRabbitMQ>();
            rabbit.InitializeAsync().Wait();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            

            app.Run();
        }
    }
}
