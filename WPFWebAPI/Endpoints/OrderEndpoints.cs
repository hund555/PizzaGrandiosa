using PizzaModels.Models;
using WPFWebAPI.Interfaces;
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
                if (orderService.CurrentOrder == null)
                    return Results.NotFound("No order available");

                orderService.CurrentOrder.IsAccepted = true;

                return Results.Ok(orderService.CurrentOrder);
            });

            
            orderApi.MapPost("/declineOrder", (IOrderService orderService) =>
            {
                if (orderService.CurrentOrder == null)
                    return Results.NotFound("No order available");

                orderService.CurrentOrder.IsAccepted = false;

                return Results.Ok(orderService.CurrentOrder);
            });

            return app;
        }
    }
}