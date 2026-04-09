
    using PizzaModels.Models;
    namespace WPFWebAPI.Endpoints
    {
        public static class OrderEndpoint
        {
            public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
            {
                var orders = new List<SalesOrder>()
            {
                new SalesOrder
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderType = "Online",
                    Date = DateTime.Now,
                    IsAccepted = false
                }
            };

                var orderApi = app.MapGroup("/api/order");

            /**
             * Endpoint to accept an order
             * Example: POST /api/order/acceptOrder/1
             */
            orderApi.MapPost("/acceptOrder/{id}", (int id) =>
                {
                    var order = orders.FirstOrDefault(o => o.Id == id);

                    if (order == null)
                        return Results.NotFound("Order not found");

                    order.IsAccepted = true;

                    return Results.Ok(order);
                });

            /**
             * DECLINE ORDER
             * This endpoint allows you to decline an order by setting its IsAccepted property to false.
             * If the order with the specified ID is not found, it returns a 404 Not Found response.
             * Otherwise, it updates the order's status and returns the updated order in the response.
             */
            orderApi.MapPost("/declineOrder/{id}", (int id) =>
                {
                    var order = orders.FirstOrDefault(o => o.Id == id);

                    if (order == null)
                        return Results.NotFound("Order not found");

                    order.IsAccepted = false;

                    return Results.Ok(order);
                });

                return app;
            }
        }
    }

