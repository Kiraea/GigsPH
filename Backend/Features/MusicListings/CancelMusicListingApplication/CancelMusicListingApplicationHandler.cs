using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.CancelMusicListingApplication;

public class CancelMusicListingApplicationHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;

    public CancelMusicListingApplicationHandler(UserManager<AppUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<CancelMusicListingApplicationResponse>> HandleAsync(
        Guid musicListingId,
        Guid applicationId,
        string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<CancelMusicListingApplicationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var found = await _dbContext.MusicListingApplications
            .Where(mla => mla.Id == applicationId
                && mla.MusicListingSlot != null
                && mla.MusicListingSlot.MusicListingId == musicListingId
                && mla.MusicListingSlot.MusicListing != null)
            .Select(mla => new
            {
                Application = mla,
                StartsAt = mla.MusicListingSlot!.MusicListing!.StartsAt
            })
            .FirstOrDefaultAsync();

        if (found == null)
        {
            return Result<CancelMusicListingApplicationResponse>.Failure(new Error
            {
                Code = "Application.NotFound",
                Message = "Application not found",
                Type = ErrorType.NotFound,
            });
        }

        var application = found.Application;
        if (application.ApplicantUserId != user.Id)
        {
            return Result<CancelMusicListingApplicationResponse>.Failure(new Error
            {
                Code = "Application.Forbidden",
                Message = "Application Not from the user",
                Type = ErrorType.Forbidden,
            });
        }

        if (found.StartsAt <= DateTime.UtcNow)
        {
            return Result<CancelMusicListingApplicationResponse>.Failure(new Error
            {
                Code = "MusicListing.AlreadyStarted",
                Message = "You can only withdraw an application before the event starts",
                Type = ErrorType.BadRequest,
            });
        }

        if (application.Status != MusicListingApplicationStatus.Pending
            && application.Status != MusicListingApplicationStatus.Accepted)
        {
            return Result<CancelMusicListingApplicationResponse>.Failure(new Error
            {
                Code = "Application.InvalidRequest",
                Message = "Application must be pending or accepted",
                Type = ErrorType.BadRequest,
            });
        }

        application.Status = MusicListingApplicationStatus.Withdrawn;
        await _dbContext.SaveChangesAsync();

        return Result<CancelMusicListingApplicationResponse>.Success(new CancelMusicListingApplicationResponse
        {
            ApplicationId = application.Id,
            SlotId = application.MusicListingSlotId,
        });
    }
}
