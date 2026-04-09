using PizzaModels.Models;

namespace WPFWebAPI.Interfaces
{
    public interface IOrderService
    {
       SalesOrder? CurrentOrder { get; set; }
    }
}
