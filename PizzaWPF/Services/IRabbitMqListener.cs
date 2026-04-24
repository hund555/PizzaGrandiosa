using System.Threading.Tasks;

namespace PizzaWPF.Services
{
    public interface IRabbitMqListener
    {
        Task StartListeningAsync();
        Task StopAsync();
    }
}