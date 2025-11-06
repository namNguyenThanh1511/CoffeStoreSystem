using Microsoft.AspNetCore.SignalR;
using PRN232.Lab2.CoffeeStore.Services.ChatService;
using PRN232.Lab2.CoffeeStore.Services.Models.Chat;

namespace PRN232.Lab2.CoffeeStore.Services.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Khi user gửi tin nhắn
        public async Task SendMessage(SendMessageRequest request)
        {
            // Lưu vào DB
            var message = await _chatService.SendMessageAsync(request);

            // Gửi realtime đến tất cả client trong cùng conversation
            await Clients.Group(request.ConversationId.ToString())
                .SendAsync("ReceiveMessage", message);
        }

        // Khi user join conversation (để nhận realtime)
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}
