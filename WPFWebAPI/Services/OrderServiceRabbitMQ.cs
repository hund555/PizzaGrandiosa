using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using WPFWebAPI.Services.Service_Interfaces;

namespace WPFWebAPI.Services
{
    public class OrderServiceRabbitMQ : IOrderServiceRabbitMQ
    {
        private readonly IHttpClientFactory _factory;
        private readonly RabbitMQConfigOption _config;
        private IConnection _connection;
        private IChannel _channel;
        public OrderServiceRabbitMQ(IOptions<RabbitMQConfigOption> options)
        {
            this._config = options.Value;
        }

        public async Task InitializeAsync()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _config.Host,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _config.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
                );
        }

        public async Task RecievedOrderID(int id)
        {
            var json = JsonSerializer.Serialize(id);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: _config.QueueName, body);
        }
    }
}
