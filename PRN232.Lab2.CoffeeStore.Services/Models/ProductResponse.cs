namespace PRN232.Lab2.CoffeeStore.Services.Models
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<CoffeeVariantResponse> Variants { get; set; } = new();
    }
}
