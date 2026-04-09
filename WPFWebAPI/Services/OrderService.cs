using PizzaModels.Models;
using WPFWebAPI.Interfaces;

namespace WPFWebAPI.Services
{
    

    public class OrderService : IOrderService
    {
        public SalesOrder? CurrentOrder { get; set; }
    }
}