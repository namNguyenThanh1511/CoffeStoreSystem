namespace PRN232.Lab2.CoffeeStore.Services.Models.Chat
{
    public class CreateConversationRequest
    {
        public Guid CustomerId { get; set; }
        public Guid BaristaId { get; set; }
        public string? Title { get; set; }
    }
}
