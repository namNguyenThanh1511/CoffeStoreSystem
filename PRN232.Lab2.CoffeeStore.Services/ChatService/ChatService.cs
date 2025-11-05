using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.Models.Chat;

namespace PRN232.Lab2.CoffeeStore.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ---------------------------
        // 1️⃣ Tạo hội thoại mới
        // ---------------------------
        public async Task<ConversationResponse> CreateConversationAsync(CreateConversationRequest request)
        {
            var conversation = new Conversation
            {
                Title = request.Title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Conversations.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();



            // Thêm 2 participant
            var participants = new List<Participant>
            {
                new() { ConversationId = conversation.Id, UserId = request.CustomerId, Role = Role.Customer.ToString(), JoinedAt = DateTime.UtcNow },
                new() { ConversationId = conversation.Id, UserId = request.BaristaId, Role = Role.Barista.ToString(), JoinedAt = DateTime.UtcNow }
            };

            foreach (var p in participants)
                await _unitOfWork.Participants.AddAsync(p);

            await _unitOfWork.SaveChangesAsync();

            return new ConversationResponse
            {
                Id = conversation.Id,
                Title = conversation.Title,
                UpdatedAt = conversation.UpdatedAt,
                Participants = participants.Select(p => new ParticipantResponse
                {
                    UserId = p.UserId,
                    Role = p.Role,
                    LastSeen = p.LastSeen,
                    Username = "" // có thể map thêm nếu query join User
                }).ToList()
            };
        }

        // ---------------------------
        // 1️⃣ Tạo hoặc lấy hội thoại của Customer
        // ---------------------------
        public async Task<ConversationResponse> GetOrCreateConversationForCustomerAsync(CreateCustomerConversationRequest request)
        {
            var customerId = request.CustomerId;
            // Lấy conversation của customer (nếu có)
            var existingParticipant = (await _unitOfWork.Participants.GetAllAsync())
                .FirstOrDefault(p => p.UserId == customerId && p.Role == Role.Customer.ToString());

            Conversation conversation;

            if (existingParticipant != null)
            {
                // Đã có -> lấy lại conversation
                conversation = await _unitOfWork.Conversations.GetByIdAsync(existingParticipant.ConversationId)
                    ?? throw new Exception("Conversation not found.");
            }
            else
            {
                // Tạo mới conversation
                conversation = new Conversation
                {
                    Title = $"Chat with Customer {customerId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Conversations.AddAsync(conversation);
                await _unitOfWork.SaveChangesAsync();

                // Thêm CUSTOMER participant
                var customerParticipant = new Participant
                {
                    ConversationId = conversation.Id,
                    UserId = customerId,
                    Role = Role.Customer.ToString(),
                    JoinedAt = DateTime.UtcNow
                };
                await _unitOfWork.Participants.AddAsync(customerParticipant);

                // Thêm sẵn ADMIN và STAFF vào phòng
                var staffAndAdmins = (await _unitOfWork.Users.GetAllAsync())
                    .Where(u => u.Role == Role.Barista || u.Role == Role.Admin)
                    .Select(u => new Participant
                    {
                        ConversationId = conversation.Id,
                        UserId = u.Id,
                        Role = u.Role.ToString(),
                        JoinedAt = DateTime.UtcNow
                    }).ToList();

                foreach (var p in staffAndAdmins)
                    await _unitOfWork.Participants.AddAsync(p);

                await _unitOfWork.SaveChangesAsync();
            }

            // Map dữ liệu trả về
            var participants = (await _unitOfWork.Participants.GetAllAsync())
                .Where(p => p.ConversationId == conversation.Id)
                .Select(p => new ParticipantResponse
                {
                    UserId = p.UserId,
                    Role = p.Role,
                    LastSeen = p.LastSeen,
                    Username = "" // có thể map thêm nếu cần
                }).ToList();

            return new ConversationResponse
            {
                Id = conversation.Id,
                Title = conversation.Title,
                UpdatedAt = conversation.UpdatedAt,
                Participants = participants
            };
        }


        // ---------------------------
        // 2️⃣ Lấy tất cả hội thoại của user
        // ---------------------------
        public async Task<IEnumerable<ConversationResponse>> GetUserConversationsAsync(Guid userId)
        {
            var participants = await _unitOfWork.Participants
                .GetAllAsync();

            var conversationIds = participants
                .Where(p => p.UserId == userId)
                .Select(p => p.ConversationId)
                .Distinct()
                .ToList();


            var conversations = await _unitOfWork.Conversations
                .GetAllAsync(c => c.Include(c => c.Messages));
            //include Sender



            var result = conversations
                .Where(c => conversationIds.Contains(c.Id))
                .Select(c => new ConversationResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    UpdatedAt = c.UpdatedAt,
                    LastMessage = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => new MessageResponse
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            Content = m.Content,
                            MessageType = m.MessageType,
                            IsRead = m.IsRead,
                            CreatedAt = m.CreatedAt,
                            SenderName = "" // có thể lấy từ User nếu cần join
                        })
                        .FirstOrDefault()
                });

            return result;
        }

        // ---------------------------
        // 3️⃣ Lấy danh sách tin nhắn
        // ---------------------------
        public async Task<IEnumerable<MessageResponse>> GetMessagesAsync(long conversationId)
        {
            var messages = await _unitOfWork.Messages.GetAllAsync(m => m.Include(m => m.Sender));
            return messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageResponse
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    MessageType = m.MessageType,
                    IsRead = m.IsRead,
                    CreatedAt = m.CreatedAt,
                    SenderName = m.Sender != null ? m.Sender.FullName : ""
                });
        }

        // ---------------------------
        // 4️⃣ Gửi tin nhắn
        // ---------------------------
        public async Task<MessageResponse> SendMessageAsync(SendMessageRequest request)
        {
            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Content = request.Content,
                MessageType = request.MessageType,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.Messages.AddAsync(message);
            var sender = await _unitOfWork.Users.GetByIdAsync(request.SenderId);

            // cập nhật conversation.updated_at
            var conversation = await _unitOfWork.Conversations.GetByIdAsync(request.ConversationId);
            if (conversation != null)
            {
                conversation.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Conversations.Update(conversation);
            }

            await _unitOfWork.SaveChangesAsync();

            return new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                Content = message.Content,
                MessageType = message.MessageType,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                SenderName = sender != null ? sender.FullName : ""
            };
        }


    }
}
