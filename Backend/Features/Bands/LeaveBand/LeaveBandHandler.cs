using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.LeaveBand;

public class LeaveBandHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public LeaveBandHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<LeaveBandResponse>> HandleAsync(Guid bandId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<LeaveBandResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var userBand = await _dbContext.UserBands
            .FirstOrDefaultAsync(ub =>
                ub.UserId == userId &&
                ub.BandId == bandId);
        if (userBand == null)
        {
            return Result<LeaveBandResponse>.Failure(new Error
            {
                Code = "Band.NotMember",
                Message = "You are not a member of this band.",
                Type = ErrorType.BadRequest,
            });
        }

        var hasOtherMembers = await _dbContext.UserBands
            .AnyAsync(ub => ub.BandId == bandId && ub.UserId != userId);

        if (userBand.Role == BandRole.Leader && hasOtherMembers)
        {
            return Result<LeaveBandResponse>.Failure(new Error
            {
                Code = "Band.LeaderMustTransfer",
                Message = "Transfer leadership to another member before leaving the band.",
                Type = ErrorType.BadRequest,
            });
        }

        _dbContext.UserBands.Remove(userBand);

        if (!hasOtherMembers)
        {
            var pendingInvitations = await _dbContext.BandInvitations
                .Where(bi => bi.BandId == bandId && bi.Status == InvitationStatus.Pending)
                .ToListAsync();

            foreach (var invitation in pendingInvitations)
            {
                invitation.Status = InvitationStatus.Cancelled;
            }
        }

        await _dbContext.SaveChangesAsync();

        return Result<LeaveBandResponse>.Success(new LeaveBandResponse()
        {
            Id = userBand.BandId,
        });
    }
}
