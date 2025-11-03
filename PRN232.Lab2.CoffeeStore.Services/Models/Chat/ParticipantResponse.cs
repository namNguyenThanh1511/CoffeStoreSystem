namespace PRN232.Lab2.CoffeeStore.Services.Models.Chat
{
    public class ParticipantResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime? LastSeen { get; set; }
    }
}
