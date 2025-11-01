namespace PRN232.Lab2.CoffeeStore.Repositories.Entities{
    public class CoffeePriceSchedule{
        public Guid Id { get; set; }
        public Guid VariantId { get; set; }
        public CoffeeVariant Variant { get; set; }
        public decimal Price { get; set; }
        //tiny int 
        public int DayMask { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;} = DateTime.UtcNow;
    }
}