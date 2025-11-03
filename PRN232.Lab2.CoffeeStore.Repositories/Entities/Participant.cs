namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class Participant
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
        public DateTime? LastSeen { get; set; }

        // Navigation
        public Conversation? Conversation { get; set; }
        public User? User { get; set; }
    }
}
