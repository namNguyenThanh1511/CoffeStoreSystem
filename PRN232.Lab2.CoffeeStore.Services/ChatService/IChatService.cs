using PRN232.Lab2.CoffeeStore.Services.Models.Chat;

namespace PRN232.Lab2.CoffeeStore.Services.ChatService
{
    public interface IChatService
    {
        Task<ConversationResponse> CreateConversationAsync(CreateConversationRequest request);
        Task<IEnumerable<ConversationResponse>> GetUserConversationsAsync(Guid userId);
        Task<IEnumerable<MessageResponse>> GetMessagesAsync(long conversationId);
        Task<MessageResponse> SendMessageAsync(SendMessageRequest request);
    }
}
