namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public bool IsRefreshTokenRevoked { get; set; } = false;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Participant>? Participants { get; set; }
        public ICollection<Message>? SentMessages { get; set; }
    }

    public enum Role
    {
        Customer = 0,
        Admin = 1,
        Barista = 2,
    }
}
