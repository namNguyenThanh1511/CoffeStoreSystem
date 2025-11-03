namespace PRN232.Lab2.CoffeeStore.Services.Models.Chat
{
    public class ConversationResponse
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ParticipantResponse> Participants { get; set; } = new();
        public List<MessageResponse> Messages { get; set; } = new();

        public MessageResponse? LastMessage { get; set; }
    }
}
