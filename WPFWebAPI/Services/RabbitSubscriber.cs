using PizzaModels.Models;
using System.Collections.Concurrent;
using System.Text.Json;


namespace WPFWebAPI.Services
{
    // Lightweight in-memory subscriber replacement (avoids compile-time RabbitMQ dependency)
    public class RabbitSubscriber : IDisposable
    {
        private readonly ConcurrentDictionary<int, DateTime> _pending = new();
        private readonly IHttpClientFactory _factory;
        private readonly IOrderService _orderService;

        public RabbitSubscriber(IHttpClientFactory factory, IConfiguration config, IOrderService orderService)
        {
            _factory = factory;
            _orderService = orderService;
        }

        public void Subscribe()
        {
            Console.WriteLine("Subscriber subscribed (in-memory)");
        }

        public void Unsubscribe()
        {
            Console.WriteLine("Subscriber unsubscribed (in-memory)");
        }

        public List<int> GetPendingOrders()
        {
            return _pending.Keys.ToList();
        }

        // Add a pending order (called by a push from the API)
        public void AddPending(int orderId)
        {
            _pending[orderId] = DateTime.UtcNow;
            Console.WriteLine($"Order {orderId} added to pending (in-memory)");
        }

        public void HandleIncomingOrder(string json)
        {
            try
            {
                var order = JsonSerializer.Deserialize<SalesOrder>(json);

                if (order == null)
                {
                    Console.WriteLine("Failed to deserialize order");
                    return;
                }

               
                _orderService.SetCurrentOrder(order);

                
                _pending[order.Id] = DateTime.UtcNow;

                Console.WriteLine($"Order {order.Id} received from MQ and stored");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing order: {ex.Message}");
            }
        }

        public async Task<bool> RespondAsync(int orderId, bool accepted)
        {
            if (!_pending.ContainsKey(orderId)) return false;

            using var client = _factory.CreateClient("Api");
            var payload = new { orderId = orderId, accepted = accepted };
            var resp = await client.PostAsJsonAsync("/api/salesorder/response", payload);
            if (resp.IsSuccessStatusCode)
            {
                _pending.TryRemove(orderId, out var _);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            // no-op for now
        }
    }
}
