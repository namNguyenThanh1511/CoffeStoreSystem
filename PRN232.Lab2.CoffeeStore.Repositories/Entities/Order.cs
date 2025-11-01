namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PROCESSING;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.UNPAID;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt {get; set;} = DateTime.UtcNow;

        public ICollection<OrderDetail> OrderItems { get; set; } = new List<OrderDetail>();
        //fk
        public Guid? CustomerId { get; set; }
        // Quan hệ N - 1 với User
        public User? Customer { get; set; }
        
        // Quan hệ 1 - N với Payment
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
    public enum PaymentStatus
    {
        PAID,
        UNPAID
    }
    public enum OrderStatus
    {
        PROCESSING,
        BREWING,
        READY,
        DELIVERING,
        COMPLETED,
        CANCELLED
    }
    public enum DeliveryType{
        PICKUP,
        DELIVERY
    }
}
