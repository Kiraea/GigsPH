using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.GetMusicListingApplications;

public class GetMusicListingApplicationsHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;

    public GetMusicListingApplicationsHandler(UserManager<AppUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<GetMusicListingApplicationsResponse>> HandleAsync(Guid musicListingId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<GetMusicListingApplicationsResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        var getMusicListingApplicationsResponse = await _dbContext.MusicListings
            .AsNoTracking()
            .Where(ml => ml.Id == musicListingId && ml.UserId == userId)
            .Select(ml => new GetMusicListingApplicationsResponse()
            {
                Slots = ml.Slots.Select(mls => new MusicListingSlotApplicantsDto()
                {
                    SlotId = mls.Id,
                    InstrumentId= mls.InstrumentId,
                    Applicants = mls.Applications.Select(mla => new MusicListingApplicantDto()
                    {
                        ApplicantUserId = mla.ApplicantUserId,
                        Note = mla.Note,
                        Status = mla.Status,
                        DisplayName = (mla.ApplicantUser == null ? null : mla.ApplicantUser.DisplayName)?? "UnknownUser",
                        ApplicationId = mla.Id,
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync();
        
        
        if (getMusicListingApplicationsResponse == null)
        {
            var listingExists = await _dbContext.MusicListings
                .AnyAsync(ml => ml.Id == musicListingId);

            return Result<GetMusicListingApplicationsResponse>.Failure(new Error
            {
                Code = listingExists ? "MusicListing.NotOwner" : "MusicListing.NotFound",
                Message = listingExists
                    ? "Only the listing owner can view its applications"
                    : "Music listing not found",
                Type = listingExists ? ErrorType.Forbidden : ErrorType.NotFound
            });
        }

        return Result<GetMusicListingApplicationsResponse>.Success(getMusicListingApplicationsResponse);
    }
}
