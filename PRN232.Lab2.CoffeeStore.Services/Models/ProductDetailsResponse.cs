namespace PRN232.Lab2.CoffeeStore.Services.Models
{
    public class ProductDetailsResponse : ProductResponse
    {
        public string? Origin { get; set; }
        public string RoastLevel { get; set; }
        public string BrewMethod { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
