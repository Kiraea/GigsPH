using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.UpdateBandMemberRole;

public class UpdateBandMemberRoleHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UpdateBandMemberRoleHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<UpdateBandMemberRoleResponse>> HandleAsync(
        Guid bandId,
        string targetUserId,
        UpdateBandMemberRoleRequest request,
        string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<UpdateBandMemberRoleResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var bandExists = await _dbContext.Bands.AnyAsync(b => b.Id == bandId);
        if (!bandExists)
        {
            return Result<UpdateBandMemberRoleResponse>.Failure(new Error
            {
                Code = "Band.NotFound",
                Message = "Band not found.",
                Type = ErrorType.NotFound,
            });
        }

        var callerMembership = await _dbContext.UserBands
            .FirstOrDefaultAsync(ub => ub.BandId == bandId && ub.UserId == userId);
        if (callerMembership == null || callerMembership.Role != BandRole.Leader)
        {
            return Result<UpdateBandMemberRoleResponse>.Failure(new Error
            {
                Code = "Band.NotLeader",
                Message = "Only the band leader can transfer leadership.",
                Type = ErrorType.Forbidden,
            });
        }

        if (targetUserId == userId)
        {
            return Result<UpdateBandMemberRoleResponse>.Failure(new Error
            {
                Code = "Band.CannotUpdateOwnRole",
                Message = "You cannot update your own band role.",
                Type = ErrorType.BadRequest,
            });
        }

        if (request.Role != BandRole.Leader)
        {
            return Result<UpdateBandMemberRoleResponse>.Failure(new Error
            {
                Code = "Band.InvalidRoleTransition",
                Message = "A leader can only transfer leadership to another band member.",
                Type = ErrorType.BadRequest,
            });
        }

        var targetMembership = await _dbContext.UserBands
            .FirstOrDefaultAsync(ub => ub.BandId == bandId && ub.UserId == targetUserId);
        if (targetMembership == null)
        {
            return Result<UpdateBandMemberRoleResponse>.Failure(new Error
            {
                Code = "Band.TargetNotMember",
                Message = "The target user is not a member of this band.",
                Type = ErrorType.NotFound,
            });
        }

        callerMembership.Role = BandRole.Member;
        targetMembership.Role = BandRole.Leader;

        await _dbContext.SaveChangesAsync();

        return Result<UpdateBandMemberRoleResponse>.Success(new UpdateBandMemberRoleResponse
        {
            BandId = bandId,
            TargetUserId = targetMembership.UserId,
            TargetRole = targetMembership.Role,
            ActingUserId = callerMembership.UserId,
            ActingRole = callerMembership.Role,
        });
    }
}
