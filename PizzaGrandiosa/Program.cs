
using Microsoft.EntityFrameworkCore;
using PizzaGrandiosa.Endpoints;
using PizzaGrandiosa.Persistence;
using PizzaGrandiosa.Repositories;
using System;
using System.Threading.Tasks;

namespace PizzaGrandiosa
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            // Configure DbContext with PostgreSQL
            builder.Services.AddDbContext<PizzaDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });

            // Register services
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
            builder.Services.AddScoped<ISalesLineRepository, SalesLineRepository>();

            var app = builder.Build();

            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                // Apply migrations automatically in Dev mode
                await using (var scope = app.Services.CreateAsyncScope())
                await using (var dbContext = scope.ServiceProvider.GetRequiredService<PizzaDbContext>())
                {
                    await dbContext.Database.EnsureCreatedAsync();
                    Console.WriteLine("migrate migrate migrate");
                }

                //Not working ??
                //https://learn.microsoft.com/en-us/ef/core/cli/dotnet
                //Seed the DB, could create an async Task for performance benefits
                //Only run this when seeding
                //using (var serviceScope = app.Services.CreateAsyncScope())
                //using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<PizzaDbContext>())
                //{
                //    dbContext.Database.EnsureCreatedAsync();
                //}
            }


            app.UseAuthorization();

            // Map endpoints
            app.MapGet("/", () => "Hello World!")
               .Produces(200, typeof(string));

            app.MapUserEndpoints();
            app.MapCustomerEndpoints();
            app.MapProductEndpoints();
            app.MapSalesOrderEndpoints();
            app.MapSalesLilneEndpoints();
 

            app.Run();
        }

     
    }
}
