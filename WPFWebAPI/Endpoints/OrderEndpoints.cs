using WPFWebAPI.Services;

namespace WPFWebAPI.Endpoints
{
    public static class OrderEndpoint
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var orderApi = app.MapGroup("/api/order");

           
            orderApi.MapPost("/acceptOrder", (IOrderService orderService) =>
            {
                var order = orderService.GetCurrentOrder();

                if (order == null)
                    return Results.NotFound("No order");

                orderService.AcceptOrder();

                return Results.Ok(order);
            });

            orderApi.MapPost("/declineOrder", (IOrderService orderService) =>
            {
                var order = orderService.GetCurrentOrder();

                if (order == null)
                    return Results.NotFound("No order");

                orderService.DeclineOrder();

                return Results.Ok(order);
            });

            return app;
        }
    }
}