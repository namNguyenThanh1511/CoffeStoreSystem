using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Order
{
    public class OrderStatusUpdateRequest
    {
        public long OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}

