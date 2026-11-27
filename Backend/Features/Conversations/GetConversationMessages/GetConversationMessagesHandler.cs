using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GetConversationMessages;

public class GetConversationMessagesHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetConversationMessagesHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<GetConversationMessagesResponse>> HandleAsync(
        Guid conversationId, string userId, DateTimeOffset? cursor)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<GetConversationMessagesResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        var pageSize = 20;
        

        
        // 10.03 <
        var isPartOfConversation =
            await _dbContext.ConversationMembers.AnyAsync(cm =>
                cm.ConversationId == conversationId && cm.UserId == userId);
        if (!isPartOfConversation)
        {
            
            return Result<GetConversationMessagesResponse>.Failure(new Error
            {
                Code = "User.CantReadMessage",
                Message = "User not part of conversation",
                Type = ErrorType.Forbidden,
            });
        }

        

        var query = _dbContext.Messages.AsNoTracking().Where(m => m.ConversationId == conversationId);
        if (cursor != null)
        {
            var cursorUtc = cursor.Value.UtcDateTime;
            query = query.Where(m => m.CreatedAt < cursorUtc);
            
        }
        var messages = await query.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id).Select(m => new MessageDto()
        {
            Id = m.Id,
            ClientMessageId = m.ClientMessageId,
            CreatedAt =  m.CreatedAt,
            SenderUserId = m.SenderUserId,
            SenderDisplayName = m.SenderUser!.DisplayName ?? "Unknown User",
            Text = m.Text,
        }).Take(pageSize + 1).ToListAsync();
        
        
        var hasMore = messages.Count > pageSize;
        if (hasMore)
        {
            messages.RemoveAt(messages.Count - 1);
        }

        messages.Reverse();

        DateTime? oldest = hasMore ? messages[0].CreatedAt : null;

        return Result<GetConversationMessagesResponse>.Success(new GetConversationMessagesResponse()
        {
            ConversationId = conversationId,
            Messages = messages,
            Cursor = oldest
        });

 
    }
}
