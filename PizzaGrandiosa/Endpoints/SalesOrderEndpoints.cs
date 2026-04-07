//using PizzaGrandiosa.Models;
using PizzaGrandiosa.Repositories;
using PizzaModels.Models;

namespace PizzaGrandiosa.Endpoints
{
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




            salesOrderApi.MapPost("/", async (SalesOrder salesOrder, ISalesOrderRepository repo) => {

                //salesOrder = new SalesOrder { 
                //    Price = 99,
                //    CustomerId = 1,
                //    Date = DateTime.Now,
                //};

                Console.WriteLine($"Post SalesOrder invoked");
                //var newSalesOrder = salesOrder;
                SalesOrder? newSalesOrder = await repo.Add(salesOrder);
                return TypedResults.Created($"/api/customer/{newSalesOrder?.Id}", newSalesOrder);
            });

        }
    }
}
