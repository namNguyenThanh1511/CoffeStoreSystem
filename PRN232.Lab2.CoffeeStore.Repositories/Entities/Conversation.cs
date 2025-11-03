namespace PRN232.Lab2.CoffeeStore.Repositories.Entities
{
    public class Conversation
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // Navigation
        public ICollection<Participant>? Participants { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
