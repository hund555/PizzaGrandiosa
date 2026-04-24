namespace WPFWebAPI.Services.Service_Interfaces
{
    public interface IOrderServiceRabbitMQ
    {
        Task InitializeAsync();
        Task RecievedOrderID(int id);
    }
}