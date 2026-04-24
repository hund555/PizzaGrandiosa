using System.Threading.Tasks;

namespace PizzaWPF.Services
{
    public interface IApiClient
    {
        Task<bool> SendAcceptAsync(string orderId);
        Task<bool> SendDeclineAsync(string orderId);
    }
}