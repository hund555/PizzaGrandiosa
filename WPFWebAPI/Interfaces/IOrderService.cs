using PizzaModels.Models;

public interface IOrderService
{
    SalesOrder? GetCurrentOrder();
    void SetCurrentOrder(SalesOrder order);
    void AcceptOrder();
    void DeclineOrder();
}