using RabbitMQ.Client;
using System.Text.Json;
using WPFWebAPI.SSE;

namespace WPFWebAPI.Services
{
    public class OrderService
    {
        List<SSEItem> httpContextItems = new List<SSEItem>();
        private readonly IHttpClientFactory _factory;
        public OrderService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task RecievedOrderID(int id)
        {

        }

        public async Task OrderStatus(int id, bool isAccepted)
        {
            using HttpClient client = _factory.CreateClient("Default");
            SSEPayload pl = new SSEPayload { IsKeepAlive = true };
            if (isAccepted)
            {
                pl.Message = "Order accepted";
            }
            else
            {
                pl.Message = "Order rejected";
            }

            var plObj = JsonSerializer.Serialize(pl);

            List<SSEItem> httpContextItemsSnapshot = null;

            lock (httpContextItems)
            {
                httpContextItemsSnapshot = httpContextItems.ToList();
            }

            foreach (var item in httpContextItemsSnapshot)
            {
                try
                {
                    await item.Context.Response.WriteAsync($"data: {plObj}\n\n");
                    await item.Context.Response.Body.FlushAsync();
                }
                catch (Exception ex)
                {
                    //There is something wrong with the httpContext, remove it
                    lock (httpContextItems)
                    {
                        httpContextItems.Remove(item);
                    }
                    Console.WriteLine("publish exception: " + ex.Message);
                }
            }
        }
    }
}
