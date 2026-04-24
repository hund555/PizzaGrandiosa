using WPFWebAPI.Services;

namespace WPFWebAPI.Endpoints
{
    public static class OrderEndpoint
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var orderApi = app.MapGroup("/api/order");


            orderApi.MapPost("/acceptOrder", async (IOrderService orderService, RabbitSubscriber subscriber) =>
            {
                var order = orderService.GetCurrentOrder();

                if (order == null)
                    return Results.NotFound("No order");

                orderService.AcceptOrder();

                await subscriber.RespondAsync(order.Id, true);

                return Results.Ok(order);
            });

            orderApi.MapPost("/declineOrder", async (IOrderService orderService, RabbitSubscriber subscriber) =>
            {
                var order = orderService.GetCurrentOrder();

                if (order == null)
                    return Results.NotFound("No order");

                orderService.DeclineOrder();

                await subscriber.RespondAsync(order.Id, false);

                return Results.Ok(order);
            });

            return app;
        }
    }
}