
using Microsoft.Extensions.Options;
using PizzaWebAPI.Endpoints;
using PizzaWebAPI.Service;

namespace PizzaWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();


            #region add CORS policy
            //const string myCors = "clientOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyAllowedOrigins",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:8082", "http://localhost")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); 

                    });
            });
            #endregion


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //Add Docker instance Uir reference
            builder.Services.AddHttpClient( 
                "Default",
                    client =>
                    {
                        client.BaseAddress = new Uri(builder.Configuration["ApiSetting:DBServerURL"]);
                    });

            builder.Services.AddHttpClient(
                "wpfapi",
                client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["ApiSetting:WPFAPI"]);
                });

            builder.Services.AddScoped<IWebServiceUser, WebServiceUser>();
            builder.Services.AddScoped<IWebServiceProduct, WebServiceProduct>();
            builder.Services.AddScoped<IWebServiceCustomer, WebServiceCustomer>();
            builder.Services.AddScoped<IWebServiceSalesLine, WebServiceSalesLine>();
            builder.Services.AddScoped<IWebServiceSalesOrder, WebServiceSalesOrder>();

            var app = builder.Build();

            app.UseCors("MyAllowedOrigins");

            //app.UseRouting();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", app.Environment.ApplicationName);
                });

            }

            
            app.UseAuthorization();
            //app.UseHttpsRedirection();

            //The backend is alive
            app.MapGet("/", () => "Hello world from the core web API");

            app.MapUserEndpoint();
            app.MapCustomerEndpoint();
            app.MapProductEndpoint();
            app.MapSalesLineEndpoint();
            app.MapSalesOrderEndpoint();


            app.Run();
        }
    }
}
