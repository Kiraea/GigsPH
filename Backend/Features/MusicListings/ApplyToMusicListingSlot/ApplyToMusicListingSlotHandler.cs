using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.ApplyToMusicListingSlot;

public class ApplyToMusicListingSlotHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;

    public ApplyToMusicListingSlotHandler(UserManager<AppUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<ApplyToMusicListingSlotResponse>> HandleAsync(Guid musicListingId, Guid slotId, ApplyToMusicListingSlotRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<ApplyToMusicListingSlotResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        
        var musicListingSlot = await _dbContext.MusicListingSlots.Include(s => s.MusicListing).FirstOrDefaultAsync(mls => mls.Id == slotId && mls.MusicListing != null && mls.MusicListing.Id == musicListingId);
        
        
        if (musicListingSlot == null || musicListingSlot.MusicListing == null)
        {
            
            return Result<ApplyToMusicListingSlotResponse>.Failure(new Error
            {
                Code = "MusicListingSlot.NotFound",
                Message = "Music listing slot not found",
                Type = ErrorType.NotFound,
            });
        }
        
        
        
       
        var timeNow = DateTime.UtcNow;
        // check if slot has already someone accepted 
        // check if musicListing expired already
        // check if user applied and pending laready
        if (musicListingSlot.MusicListing.ExpiresAt < timeNow || musicListingSlot.MusicListing.StartsAt <= timeNow)
        {
            
            return Result<ApplyToMusicListingSlotResponse>.Failure(new Error
            {
                Code = "MusicListingSlot.AlreadyExpired",
                Message = "Slot is already expired",
                Type = ErrorType.NotFound,
            });
        }

        var isDuplicate = await _dbContext.MusicListingApplications.AnyAsync(mla => mla.MusicListingSlotId ==slotId 
                                                                  && (mla.Status == MusicListingApplicationStatus
                                                                      .Pending || mla.Status == MusicListingApplicationStatus.Accepted) && mla.ApplicantUserId == userId);

        if (isDuplicate)
        {
            
            return Result<ApplyToMusicListingSlotResponse>.Failure(new Error
            {
                Code = "MusicListingSlot.Duplicate",
                Message = "You already are accepted or have applied",
                Type = ErrorType.BadRequest,
            });
        }
        
        var isSomeoneAccepted = await _dbContext.MusicListingApplications.AnyAsync(mla => mla.MusicListingSlotId == slotId && mla.Status == MusicListingApplicationStatus.Accepted);
        if (isSomeoneAccepted)
        {
            
            return Result<ApplyToMusicListingSlotResponse>.Failure(new Error
            {
                Code = "MusicListingSlot.SomeoneAccepted",
                Message = "Someone has already been accepted in the slot, wait until the slot is free again",
                Type = ErrorType.BadRequest,
            });
        }

        var musicListingApplication = new MusicListingApplication()
        {
            MusicListingSlotId = slotId,
            ApplicantUserId = userId,
            Status = MusicListingApplicationStatus.Pending,
            Note = request.Note,
        };
        
        _dbContext.Add(musicListingApplication);
        musicListingSlot.Applications.Add(musicListingApplication);
        await _dbContext.SaveChangesAsync();
        
        return Result<ApplyToMusicListingSlotResponse>.Success(
            new ApplyToMusicListingSlotResponse
            {
                ApplicationId = musicListingApplication.Id,
                SlotId = slotId
            });
        

    }
}
