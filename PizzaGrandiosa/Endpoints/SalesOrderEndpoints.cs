//using PizzaGrandiosa.Models;
using PizzaGrandiosa.Repositories;
using PizzaModels.Models;

namespace PizzaGrandiosa.Endpoints
{
    public record ResponseDto(int OrderId, bool Accepted);

    public static class SalesOrderEndpoints
    {
        public static void MapSalesOrderEndpoints(this IEndpointRouteBuilder routes)
        {
            var salesOrderApi = routes.MapGroup("/api/salesorder").WithTags("salesorder");

            salesOrderApi.MapGet("/{id}", async (ISalesOrderRepository repo, int id) =>
            {
                Console.WriteLine($"Get salesOrder by id invoked");

                var salesOrder = await repo.Get(id);
                return TypedResults.Ok(salesOrder);
            });

            salesOrderApi.MapGet("/", async (ISalesOrderRepository repo) =>
            {
                Console.WriteLine($"Get all salesOrder invoked");

                var salesOrder = await repo.GetAllSalesOrdersAsync();
                return TypedResults.Ok(salesOrder);
            });

            salesOrderApi.MapPost("/", async (SalesOrder salesOrder, ISalesOrderRepository repo) =>
            {
                Console.WriteLine($"Post SalesOrder invoked");
                SalesOrder? newSalesOrder = await repo.Add(salesOrder);
                return TypedResults.Created($"/api/customer/{newSalesOrder?.Id}", newSalesOrder);
            });

            // Endpoint to receive accept/decline responses from subscriber apps
            salesOrderApi.MapPost("/response", HandleResponse);
        }

        private static async Task<IResult> HandleResponse(ResponseDto dto, ISalesOrderRepository repo)
        {
            if (dto == null) return TypedResults.BadRequest();

            Console.WriteLine($"Received response for order {dto.OrderId} accepted={dto.Accepted}");

            var order = await repo.Get(dto.OrderId);
            if (order == null) return TypedResults.NotFound(dto.OrderId);

            return TypedResults.Ok(new { dto.OrderId, dto.Accepted });
        }
    }
}
