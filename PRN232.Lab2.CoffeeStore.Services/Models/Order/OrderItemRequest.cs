using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Order
{
    public class OrderItemRequest
    {
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
        public Temperature Temperature { get; set; }
        public Sweetness Sweetness { get; set; }
        public MilkType MilkType { get; set; }
        public List<Guid> AddonIds { get; set; }
    }
}
