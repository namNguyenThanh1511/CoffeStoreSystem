namespace PRN232.Lab2.CoffeeStore.Repositories.Entities{
    public class OrderItemAddon{
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public Guid AddonId { get; set; }
        public CoffeeAddon Addon { get; set; }
        public decimal Price { get; set; }
    }
}