namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class OrderDetail
    {
        public Guid Id { get; set; }
        //FK
        public long OrderId { get; set; }
        //N - 1 , 1 order has many order details , 1 order detail belongs to 1 order
        public Order Order { get; set; }
        //FK
        public Guid VariantId { get; set; }
        //N - 1 , 1 variant has many order details , 1 order detail belongs to 1 variant
        public CoffeeVariant Variant { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }

        public Temperature Temperature { get; set; }
        public Sweetness Sweetness { get; set; }
        public MilkType MilkType { get; set; }
        public ICollection<OrderItemAddon> OrderItemAddons { get; set; } = new List<OrderItemAddon>();


    }
    public enum Temperature
    {
        Hot,
        ColdBrew,
        Ice
    }
    public enum Sweetness
    {
        Sweet,
        Normal,
        Less,
        NoSugar
    }
    public enum MilkType{
        Dairy,
        Condensed,
        Plant,
        None
    }
}
