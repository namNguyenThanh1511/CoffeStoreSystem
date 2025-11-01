using PRN232.Lab2.CoffeeStore.Services.Models.Product;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Order
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; }

        public ProductWithVariantResponse? ProductsWithVariant { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
        public string Temperature { get; set; }
        public string Sweetness { get; set; }
        public string MilkType { get; set; }
        public List<CoffeeAddonResponse> Addons { get; set; } = new List<CoffeeAddonResponse>();
    }
}
