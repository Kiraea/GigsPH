using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.GetGroupDetails;

public class GetGroupDetailsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetGroupDetailsHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<GetGroupDetailsResponse>> HandleAsync(
        Guid conversationId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<GetGroupDetailsResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var groupConversations = await _dbContext.Conversations.Where(m => m.Id == conversationId
                                                                           && m.Type == ConversationType.Group
                                                                           && m.ConversationMembers.Any(cm =>
                                                                               cm.UserId == userId)
        ).Select(c => new GetGroupDetailsResponse()
        {
            ConversationId = conversationId,
            Name = c.Name,
            IsOwner = c.OwnerUserId == userId ? true : false,
            UserGroupChatDetails = c.ConversationMembers.Select(c => new UserGroupChatDetail()
            {
                DisplayName = (c.User == null ? null  : c.User.DisplayName) ?? "Unknown user",
                UserId = c.User.Id,

            }).ToList()

        }).FirstOrDefaultAsync();

        if (groupConversations == null)
        {
            return Result<GetGroupDetailsResponse>.Failure(new Error
            {
                Code = "Conversation.NotFound",
                Message = "Group conversation not found.",
                Type = ErrorType.NotFound,
            });
        }

        return Result<GetGroupDetailsResponse>.Success(groupConversations);
        
 
    }
}
