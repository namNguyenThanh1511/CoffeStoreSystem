using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Services.Models
{
    public class ProductUpdationRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public RoastLevel? RoastLevel { get; set; }
        public BrewMethod? BrewMethod { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
