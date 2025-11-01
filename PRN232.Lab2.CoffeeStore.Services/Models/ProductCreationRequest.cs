using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models
{
    public class ProductCreationRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public RoastLevel RoastLevel { get; set; }
        public BrewMethod BrewMethod { get; set; }
        public string? ImageUrl { get; set; }
        public List<VariantRequest> Variants { get; set; } = new();

        public class VariantRequest
        {
            [Required]
            public CoffeeSize Size { get; set; }
            [Required]
            public decimal BasePrice { get; set; }
        }
    }
}
