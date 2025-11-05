using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Models;
using PRN232.Lab2.CoffeeStore.Services.ChatService;
using PRN232.Lab2.CoffeeStore.Services.Models.Chat;
using PRN232.Lab2.CoffeeStore.Services.UserService;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly ICurrentUserService _currentUserService;

        public ChatController(IChatService chatService, ICurrentUserService currentUserService)
        {
            _chatService = chatService;
            _currentUserService = currentUserService;
        }

        [HttpPost("conversation")]
        public async Task<ActionResult<ApiResponse<ConversationResponse>>> CreateConversation([FromBody] CreateConversationRequest request)
        {
            var result = await _chatService.CreateConversationAsync(request);
            return Ok(result);
        }

        [HttpPost("conversation/customer")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<ConversationResponse>>> CreateCustomerConversation()
        {
            var request = new CreateCustomerConversationRequest
            {
                CustomerId = Guid.Parse(_currentUserService.GetUserId())
            };
            var result = await _chatService.GetOrCreateConversationForCustomerAsync(request);
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
