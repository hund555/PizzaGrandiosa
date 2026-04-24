using PizzaModels.Models;

namespace WPFWebAPI.Services
{


    public class OrderService : IOrderService
    {
        private SalesOrder? _currentOrder;

        public SalesOrder? GetCurrentOrder()
        {
            return _currentOrder;
        }

        public void SetCurrentOrder(SalesOrder order)
        {
            _currentOrder = order;
        }

        public void AcceptOrder()
        {
            if (_currentOrder != null)
            {
                _currentOrder.IsAccepted = true;
            }
        }

        public void DeclineOrder()
        {
            if (_currentOrder != null)
            {
                _currentOrder.IsAccepted = false;
            }
        }
    }
}