using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PizzaWPF.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient http, ILogger<ApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<bool> SendAcceptAsync(string orderId)
        {
            return await SendStatusAsync(orderId, "accept");
        }

        public async Task<bool> SendDeclineAsync(string orderId)
        {
            return await SendStatusAsync(orderId, "decline");
        }

        private async Task<bool> SendStatusAsync(string orderId, string action)
        {
            try
            {
                var payload = new { OrderId = orderId };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _http.PostAsync($"/orders/{action}", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API returned {StatusCode} for {Action} {OrderId}", response.StatusCode, action, orderId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send {Action} for {OrderId}", action, orderId);
                return false;
            }
        }
    }
}