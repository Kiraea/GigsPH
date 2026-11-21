using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.AcceptMusicListingApplicant;

public class AcceptMusicListingApplicantHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;

    public AcceptMusicListingApplicantHandler(UserManager<AppUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext; 
    }

    public async Task<Result<AcceptMusicListingApplicantResponse>> HandleAsync(
        Guid musicListingId, Guid applicationId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }


        var musicListing=
            await _dbContext.MusicListings.FirstOrDefaultAsync(ml => ml.Id == musicListingId);
        
        
        if (musicListing== null)
        {
            
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "MusicListing.NotFound",
                Message = "Music Listing not found",
                Type = ErrorType.NotFound,
            });
        }

        if (musicListing.UserId != userId)
        {
            
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "User.Forbidden",
                Message = "User does not have permissions to accept",
                Type = ErrorType.Forbidden,
            });
        }
        
        var application = await _dbContext.MusicListingApplications.FirstOrDefaultAsync(mla => mla.MusicListingSlot != null
            && mla.MusicListingSlot.MusicListingId == musicListingId
            && mla.Id == applicationId);


        if (application == null)
        {
            
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "MusicListingApplication.NotFound",
                Message = "Music Listing application not found",
                Type = ErrorType.NotFound,
            });
        }

        var slotId= application.MusicListingSlotId;

        var alreadyAccepted = await _dbContext.MusicListingSlots.AnyAsync(mls => mls.Id == slotId &&
            mls.Applications.Any(a =>
                a.Status == MusicListingApplicationStatus.Accepted));

        if (alreadyAccepted)
        {
            
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "MusicListingSlot.AlreadyFilled",
                Message = "Slot already Filled by someone",
                Type = ErrorType.BadRequest,
            });
        }

        if (application.Status != MusicListingApplicationStatus.Pending)
        {
               
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "MusicListingSlot.NotPending",
                Message = "Slot needs to be Pending",
                Type = ErrorType.BadRequest,
            });
        }

        if (musicListing.StartsAt <= DateTime.UtcNow)
        {
            
            return Result<AcceptMusicListingApplicantResponse>.Failure(new Error
            {
                Code = "MusicListing.AlreadyStarted",
                Message = "Can only accept applications when event has not started yet",
                Type = ErrorType.BadRequest,
            });
        }
        
        application.Status = MusicListingApplicationStatus.Accepted;
        await _dbContext.SaveChangesAsync();
        

        return Result<AcceptMusicListingApplicantResponse>.Success(new AcceptMusicListingApplicantResponse()
        {
            ApplicationId = applicationId,
            SlotId = slotId,
        });
        


    }
}
