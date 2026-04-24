using WPFWebAPI.DTO;
using WPFWebAPI.Services;
using WPFWebAPI.Services.Service_Interfaces;


namespace WPFWebAPI.Endpoints
{
    public static class OrderEndpoint
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var orderApi = app.MapGroup("/api/order");

            orderApi.MapPost("/updateStatus", async (
                UpdateOrderStatusDTO dto,
                OrderService orderService,
                SseConnectionManager manager) =>
            {
                try
                {
                    //Opdater via service
                    orderService.UpdateOrderStatus(dto.OrderId, dto.Accepted);

                    //Lav payload
                    var payload = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        orderId = dto.OrderId,
                        accepted = dto.Accepted
                    });

                    //Send til SSE client
                    foreach (var client in manager.GetAll())
                    {
                        try
                        {
                            await client.WriteAsync($"data: {payload}\n\n");
                            await client.Body.FlushAsync();
                        }
                        catch
                        {

                        }
                    }

                    return Results.Ok(orderService.CurrentOrder);
                }
                catch
                {
                    return Results.NotFound("Order not found");
                }
            });

            app.MapPost("/createdorder", async (int id, IOrderServiceRabbitMQ orderService) =>
            {
                try
                {
                    await orderService.RecievedOrderID(id);
                    return Results.Created();
                }
                catch (Exception e)
                {
                    return Results.Problem(detail: e.Message);
                }
            });

            app.MapGet("/api/order/stream", async (
                HttpContext context,
                SseConnectionManager manager) =>
            {
              
                context.Response.Headers.Append("Content-Type", "text/event-stream");
                context.Response.Headers.Append("Cache-Control", "no-cache");
                context.Response.Headers.Append("Connection", "keep-alive");

                var id = manager.Add(context.Response);

                try
                {
                    while (!context.RequestAborted.IsCancellationRequested)
                    {
                        await Task.Delay(1000);
                    }
                }
                finally
                {
                    manager.Remove(id);
                }
            });
            return app;
        }
    }
}