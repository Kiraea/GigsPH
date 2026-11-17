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
        Guid conversationId, GetConversationMessagesRequest request, string userId)
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
        // add take next time but no need
        var messages = await _dbContext.Messages.AsNoTracking().Where(m => m.ConversationId == conversationId).OrderBy(m => m.CreatedAt).ThenBy(m => m.Id).Select(m => new MessageDto()
        {
            Id = m.Id,
            ClientMessageId = m.ClientMessageId,
            CreatedAt =  m.CreatedAt,
            SenderUserId = m.SenderUserId,
            SenderDisplayName = m.SenderUser!.DisplayName ?? "Unknown User",
            Text = m.Text,
        }).ToListAsync();

        return Result<GetConversationMessagesResponse>.Success(new GetConversationMessagesResponse()
        {
            ConversationId = conversationId,
            Messages = messages 
        });

 
    }
}
