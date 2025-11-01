namespace PRN232.Lab2.CoffeeStore.Services.Models.Product
{
    public class ProductWithVariantResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid VariantId { get; set; }
        public string VariantSize { get; set; }
        public decimal BasePrice { get; set; }
    }
}
