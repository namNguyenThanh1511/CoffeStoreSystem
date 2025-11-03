namespace PRN232.Lab2.CoffeeStore.Services.Models.Chat
{
    public class MessageResponse
    {
        public long Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string MessageType { get; set; } = "TEXT";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
