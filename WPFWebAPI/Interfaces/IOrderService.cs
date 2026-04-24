using PizzaModels.Models;

public interface IOrderService
{
    SalesOrder? CurrentOrder { get; set; }
    void UpdateOrderStatus(int orderId, bool accepted);
}