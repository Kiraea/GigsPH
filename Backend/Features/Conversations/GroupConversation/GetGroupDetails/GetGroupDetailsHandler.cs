using Microsoft.AspNetCore.Identity;
using PrjA.Common;
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
        GetGroupDetailsRequest request, string userId)
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

        // TODO: Require membership in the requested Group conversation.
        // TODO: Project its name, owner, and accepted members into response DTOs.
        // TODO: Pending invitees are not members and cannot read group history.
        throw new NotImplementedException();
    }
}

