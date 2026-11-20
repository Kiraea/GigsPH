using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.UpdateGroup;

public class UpdateGroupHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UpdateGroupHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<UpdateGroupResponse>> HandleAsync(
        Guid conversationId, UpdateGroupRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<UpdateGroupResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        var conversation = await _dbContext.Conversations.FirstOrDefaultAsync(c =>
            c.Id == conversationId &&
            c.Type == ConversationType.Group &&
            c.ConversationMembers.Any(cm => cm.UserId == userId));

        if (conversation == null)
        {
            return Result<UpdateGroupResponse>.Failure(new Error
            {
                Code = "Conversation.NotFound",
                Message = "Group conversation not found.",
                Type = ErrorType.NotFound,
            });
        }

        if (conversation.OwnerUserId != userId)
        {
            return Result<UpdateGroupResponse>.Failure(new Error
            {
                Code = "Conversation.Forbidden",
                Message = "Only the group owner can rename this group.",
                Type = ErrorType.Forbidden,
            });
        }

        var name = request.Name.Trim();
        conversation.Name = name;
        await _dbContext.SaveChangesAsync();

        return Result<UpdateGroupResponse>.Success(new UpdateGroupResponse
        {
            ConversationId = conversation.Id,
            Name = name,
        });


    }
}

