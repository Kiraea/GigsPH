using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.GetGroupInvitations;

public class GetGroupInvitationsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetGroupInvitationsHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<List<GetGroupInvitationResponse>>> HandleAsync(
        GetGroupInvitationsRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<List<GetGroupInvitationResponse>>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var invitations = await _dbContext.GroupChatInvitations
            .AsNoTracking()
            .Where(gci => gci.InvitedToUserId == user.Id &&
                          gci.Status == GroupChatInvitationStatus.Pending &&
                          gci.Conversation != null)
            .Select(gci => new GetGroupInvitationResponse
            {
                InvitationId = gci.Id,
                GroupName = gci.Conversation!.Name ?? "Group Chat",
            })
            .ToListAsync();

        return Result<List<GetGroupInvitationResponse>>.Success(invitations);
    }
}

