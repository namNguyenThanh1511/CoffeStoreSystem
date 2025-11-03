namespace PRN232.Lab2.CoffeeStore.Services.Models.User
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
