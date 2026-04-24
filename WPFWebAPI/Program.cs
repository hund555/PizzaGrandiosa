
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

            // Http client used to call back the main API when responding to orders
            builder.Services.AddHttpClient("Api", client => {
                var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";
                client.BaseAddress = new Uri(apiBase);
            });

            // Register Rabbit subscriber service
            builder.Services.AddSingleton<Services.RabbitSubscriber>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Subscriber endpoints to control subscription and respond to orders
            var subGroup = app.MapGroup("/api/subscriber").WithTags("subscriber");

            subGroup.MapPost("/subscribe", (Services.RabbitSubscriber subscriber) =>
            {
                subscriber.Subscribe();
                return Results.Ok(new { subscribed = true });
            });

            subGroup.MapPost("/unsubscribe", (Services.RabbitSubscriber subscriber) =>
            {
                subscriber.Unsubscribe();
                return Results.Ok(new { subscribed = false });
            });

            subGroup.MapGet("/pending", (Services.RabbitSubscriber subscriber) =>
            {
                var pending = subscriber.GetPendingOrders();
                return Results.Ok(pending);
            });

            subGroup.MapPost("/respond", async (int orderId, bool accepted, Services.RabbitSubscriber subscriber) =>
            {
                var ok = await subscriber.RespondAsync(orderId, accepted);
                if (!ok) return Results.NotFound(new { orderId });
                return Results.Ok(new { orderId, accepted });
            });

            

            app.Run();
        }
    }
}
