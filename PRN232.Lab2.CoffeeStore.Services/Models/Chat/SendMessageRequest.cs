namespace PRN232.Lab2.CoffeeStore.Services.Models.Chat
{
    public class SendMessageRequest
    {
        public long ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public string MessageType { get; set; } = "TEXT";
    }
}
