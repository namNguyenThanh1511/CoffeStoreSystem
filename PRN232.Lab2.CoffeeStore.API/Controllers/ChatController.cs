using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Models;
using PRN232.Lab2.CoffeeStore.Services.ChatService;
using PRN232.Lab2.CoffeeStore.Services.Models.Chat;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("conversation")]
        public async Task<ActionResult<ApiResponse<ConversationResponse>>> CreateConversation([FromBody] CreateConversationRequest request)
        {
            var result = await _chatService.CreateConversationAsync(request);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<List<ConversationResponse>>>> GetUserConversations(Guid userId)
        {
            var result = await _chatService.GetUserConversationsAsync(userId);
            return Ok(result.ToList());
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<ActionResult<ApiResponse<List<MessageResponse>>>> GetMessages(long conversationId)
        {
            var result = await _chatService.GetMessagesAsync(conversationId);
            return Ok(result.ToList());
        }

        [HttpPost("send")]
        public async Task<ActionResult<ApiResponse<MessageResponse>>> SendMessage([FromBody] SendMessageRequest request)
        {
            var result = await _chatService.SendMessageAsync(request);
            return Ok(result);
        }
    }
}
