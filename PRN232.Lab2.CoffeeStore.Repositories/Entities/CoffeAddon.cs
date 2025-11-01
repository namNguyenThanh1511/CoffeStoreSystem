namespace PRN232.Lab2.CoffeeStore.Repositories.Entities{
    public class CoffeeAddon{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;} = DateTime.UtcNow;
    }
}