using PizzaModels.Models;

public class OrderService : IOrderService
{
    public SalesOrder? CurrentOrder { get; set; }

    public void UpdateOrderStatus(int orderId, bool accepted)
    {
        if (CurrentOrder == null || CurrentOrder.Id != orderId)
            throw new Exception("Order not found");

        CurrentOrder.IsAccepted = accepted;
    }

    
}