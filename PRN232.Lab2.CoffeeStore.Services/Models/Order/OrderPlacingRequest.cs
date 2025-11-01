using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Order
{
    public class OrderPlacingRequest
    {
        public DeliveryType DeliveryType { get; set; }
        
        public List<OrderItemRequest> OrderItems { get; set; }
    }
}
