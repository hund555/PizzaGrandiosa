using WPFWebAPI.DTO;
using WPFWebAPI.Services;


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
                    // 1. Opdater via service
                    orderService.UpdateOrderStatus(dto.OrderId, dto.Accepted);

                    // 2. Lav payload
                    var payload = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        orderId = dto.OrderId,
                        accepted = dto.Accepted
                    });

                    // 3. Send til ALLE SSE clients
                    foreach (var client in manager.GetAll())
                    {
                        try
                        {
                            await client.WriteAsync($"data: {payload}\n\n");
                            await client.Body.FlushAsync();
                        }
                        catch
                        {
                            // ignore dead clients
                        }
                    }

                    return Results.Ok(orderService.CurrentOrder);
                }
                catch
                {
                    return Results.NotFound("Order not found");
                }
            });

            app.MapGet("/api/order/stream", async (
                HttpContext context,
                SseConnectionManager manager) =>
            {
                context.Response.Headers.Add("Content-Type", "text/event-stream");
                context.Response.Headers.Add("Cache-Control", "no-cache");
                context.Response.Headers.Add("Connection", "keep-alive");

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