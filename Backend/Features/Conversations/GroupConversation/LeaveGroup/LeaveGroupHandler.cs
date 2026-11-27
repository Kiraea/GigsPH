using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.LeaveGroup;

public class LeaveGroupHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public LeaveGroupHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<LeaveGroupResponse>> HandleAsync(
        Guid conversationId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<LeaveGroupResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var conversationMember = await _dbContext.ConversationMembers.Where(cm =>
            cm.ConversationId == conversationId && user.Id == cm.UserId &&
            cm.Conversation != null && cm.Conversation.Type == ConversationType.Group).FirstOrDefaultAsync();

        if (conversationMember == null)
        {
            
            return Result<LeaveGroupResponse>.Failure(new Error
            {
                Code = "Conversation.Notfound",
                Message = "User not in conversation",
                Type = ErrorType.NotFound,
            });
        }

        var isOwner =
            await _dbContext.Conversations.AnyAsync(c => c.Id == conversationId 
                                                         && c.OwnerUserId == user.Id);
        
        if (isOwner)
        {
            
            return Result<LeaveGroupResponse>.Failure(new Error
            {
                Code = "Conversation.Invalid",
                Message = "User must transfer ownership first",
                Type = ErrorType.BadRequest,
            });
        }
        
        _dbContext.ConversationMembers.Remove(conversationMember);
        await _dbContext.SaveChangesAsync();
        
        return Result<LeaveGroupResponse>.Success(new LeaveGroupResponse
        {
            ConversationId = conversationMember.ConversationId,
        });
        

    }
}
