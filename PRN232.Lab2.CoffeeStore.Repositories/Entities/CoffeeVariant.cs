namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class CoffeeVariant
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public CoffeeSize Size { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual Product? Product { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum CoffeeSize
    {
        S = 0,
        M = 1,
        L = 2
    }
}


