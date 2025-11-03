namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class Message
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public string MessageType { get; set; } = "TEXT";
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Conversation? Conversation { get; set; }
        public User? Sender { get; set; }
    }
}
