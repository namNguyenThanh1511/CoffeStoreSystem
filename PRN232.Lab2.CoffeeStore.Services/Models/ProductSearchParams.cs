using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PRN232.Lab2.CoffeeStore.Services.Models
{
    public class ProductSearchParams : RequestParams
    {
        public string Search { get; set; }
        public string SortBy { get; set; } = "Name"; // Default sort by Name
        public string SortOrder { get; set; } = "asc"; // Default sort order ascending
        public RoastLevel? RoastLevel { get; set; } // Filter by roast level
        public BrewMethod? BrewMethod { get; set; } // Filter by brew method
        public string? Origin { get; set; } // Filter by origin
        public bool? IsActive { get; set; } // Filter by active status

        public List<ProductSelectField> SelectFields { get; set; } = new List<ProductSelectField>();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ProductSelectField
        {
            [Description("Name")]
            Name,
            [Description("Description")]
            Description,
            [Description("Origin")]
            Origin,
            [Description("RoastLevel")]
            RoastLevel,
            [Description("BrewMethod")]
            BrewMethod,
            [Description("ImageUrl")]
            ImageUrl,
            [Description("IsActive")]
            IsActive,
            [Description("CreatedAt")]
            CreatedAt,
            [Description("Variants")]
            Variants,
        }
    }
}

