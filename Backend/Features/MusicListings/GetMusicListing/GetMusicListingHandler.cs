using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Features.Genres;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.GetMusicListing;

public class GetMusicListingHandler
{
    private readonly AppDbContext _dbContext;

    public GetMusicListingHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<MusicListingDto>> HandleAsync(Guid musicListingId)
    {

        var musicListingsWithSlots = await _dbContext.MusicListings.Where(ml => ml.Id == musicListingId)
            .Select(ml => new MusicListingDto()
            {
                Id = ml.Id,
                City = ml.City,
                ExpiresAt =  ml.ExpiresAt,
                StartsAt = ml.StartsAt,
                EndsAt = ml.EndsAt,
                Description = ml.Description,
                Genre = ml.Genre == null ? null : new GenreDto()
                {
                    Id = ml.Genre.Id,
                    Name = ml.Genre.Name,
                },
                Title =  ml.Title,
                Slots = ml.Slots.Select(mls => new MusicListingSlotDto()
                {
                    InstrumentId = mls.InstrumentId,
                    Id = mls.Id,
                    IsFilled = mls.Applications.Any(mla => mla.Status == MusicListingApplicationStatus.Accepted) ,
                    Note =  mls.Note,
                    
                }).ToList()
                
            }).FirstOrDefaultAsync();

        if (musicListingsWithSlots == null)
        {
            return Result<MusicListingDto>.Failure(new Error
            {
                Code = "MusicListing.NotFound",
                Message = "Music listing does not exist ",
                Type = ErrorType.NotFound,
            });
        }

        await _dbContext.SaveChangesAsync();

        return Result<MusicListingDto>.Success(musicListingsWithSlots);



    }
}
