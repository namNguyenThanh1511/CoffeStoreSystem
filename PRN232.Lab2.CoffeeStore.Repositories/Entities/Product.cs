namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Origin {get; set;}
        public RoastLevel RoastLevel {get; set;}
        public BrewMethod BrewMethod {get; set;}
        public string? ImageUrl {get; set;}
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;} = DateTime.UtcNow;

        //1 - N
        public ICollection<CoffeeVariant> Variants { get; set; } = new List<CoffeeVariant>();
    }

    public enum RoastLevel{
        Light,
        Medium,
        Dark
    }
    public enum BrewMethod{
        Espresso,
        Phin,
        PourOver,
        ColdBrew,
    }
}
