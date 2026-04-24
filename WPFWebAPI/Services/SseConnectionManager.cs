using System.Collections.Concurrent;

namespace WPFWebAPI.Services
{
    public class SseConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, HttpResponse> _clients = new();

        public Guid Add(HttpResponse response)
        {
            var id = Guid.NewGuid();
            _clients[id] = response;
            return id;
        }

        public void Remove(Guid id)
        {
            _clients.TryRemove(id, out _);
        }

        public IEnumerable<HttpResponse> GetAll() => _clients.Values;
    }
}