using System.Collections.Concurrent;
using System.Text.Json;

namespace WPFWebAPI.Services
{
    // Lightweight in-memory subscriber replacement (avoids compile-time RabbitMQ dependency)
    public class RabbitSubscriber : IDisposable
    {
        private readonly ConcurrentDictionary<int, DateTime> _pending = new();
        private readonly IHttpClientFactory _factory;

        public RabbitSubscriber(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
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
