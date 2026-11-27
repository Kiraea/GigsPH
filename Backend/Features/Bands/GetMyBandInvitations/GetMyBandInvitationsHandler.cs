using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.GetMyBandInvitations;

public class GetMyBandInvitationsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetMyBandInvitationsHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<List<GetMyBandInvitationResponse>>> HandleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<List<GetMyBandInvitationResponse>>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var invitations = await (
            from invitation in _dbContext.BandInvitations.AsNoTracking()
            where invitation.InvitedToUserId == user.Id &&
                  invitation.Status == InvitationStatus.Pending
            join band in _dbContext.Bands.AsNoTracking()
                on invitation.BandId equals band.Id
            join inviter in _dbContext.AppUsers.AsNoTracking()
                on invitation.InvitedByUserId equals inviter.Id
            orderby invitation.CreatedAt descending
            select new GetMyBandInvitationResponse
            {
                InvitationId = invitation.Id,
                BandId = band.Id,
                BandName = band.Name,
                BandDescription = band.Description,
                MemberCount = _dbContext.UserBands.Count(ub => ub.BandId == band.Id),
                InvitedByUserId = inviter.Id,
                InvitedByDisplayName = inviter.DisplayName ?? inviter.UserName ?? "Musician",
                CreatedAt = invitation.CreatedAt,
            }
        ).ToListAsync();

        return Result<List<GetMyBandInvitationResponse>>.Success(invitations);
    }
}
