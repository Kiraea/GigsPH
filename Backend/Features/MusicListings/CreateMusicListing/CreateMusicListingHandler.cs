using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.CreateMusicListing;

public class CreateMusicListingHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;

    public CreateMusicListingHandler(UserManager<AppUser> userManager, AppDbContext dbContext )
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<CreateMusicListingResponse>> HandleAsync(CreateMusicListingRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<CreateMusicListingResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        
        // limit to 5 posts
        int limit = 5;
        // do this after done witha
        
        var timeNow = DateTime.UtcNow;
        var isExceedLimit = await _dbContext.MusicListings.CountAsync(ml => ml.UserId == userId && timeNow < ml.ExpiresAt);
        if (isExceedLimit >= limit)
        {
            
            return Result<CreateMusicListingResponse>.Failure(new Error
            {
                Code = "MusicListing.LimitExceeded",
                Message = "Can only create 5 musiclisting maximum",
                Type = ErrorType.BadRequest,
            });
        }

        var startsAt = request.StartsAt.ToUniversalTime();
        var endsAt = request.EndsAt.ToUniversalTime();
        var expiresAt = request.ExpiresAt.ToUniversalTime();

        if (expiresAt <= timeNow || expiresAt >startsAt)
        {
            
            return Result<CreateMusicListingResponse>.Failure(new Error
            {
                Code = "MusicListing.InvalidExpiresAt",
                Message = "InvalidAt must be in the future",
                Type = ErrorType.BadRequest,
            });
        }
        
        
        if (startsAt < timeNow)
        {
            
            return Result<CreateMusicListingResponse>.Failure(new Error
            {
                Code = "MusicListing.InvalidStartsAt",
                Message = "Starts At must be in the future",
                Type = ErrorType.BadRequest,
            });
        }
        
        if (endsAt <= startsAt)
        {
            return Result<CreateMusicListingResponse>.Failure(new Error
            {
                Code = "MusicListing.InvalidEndsAt",
                Message = "Invalid must be in the future",
                Type = ErrorType.BadRequest,
            });
        }
        
        

        if (request.GenreId != null)
        {
            var genre = await _dbContext.Genres.FindAsync(request.GenreId);
            if (genre == null)
            {
                
                return Result<CreateMusicListingResponse>.Failure(new Error
                {
                    Code = "Genre.NotExist",
                    Message = "Genre does not Exist",
                    Type = ErrorType.NotFound,
                });
            }
        }
        
        // validate city but this is next time no need now
        
        var musicListing = new MusicListing()
        {
            Title = request.Title,
            Description = request.Description,
            ExpiresAt = expiresAt,
            City = request.City,
            UserId =  userId,
            GenreId = request.GenreId ,
            StartsAt = startsAt,
            EndsAt = endsAt,
        };

        if (request.MusicListingSlotDto.Count == 0)
        {
            return Result<CreateMusicListingResponse>.Failure(new Error
            {
                Code = "MusicListing.NoSlotCreated",
                Message = "Music listing needs to have a slot",
                Type = ErrorType.BadRequest,
            });
        }


        foreach (var slot in request.MusicListingSlotDto)
        {
            
            var instrumentExist = await _dbContext.Instruments.AnyAsync(i => i.Id == slot.InstrumentId);
            if (!instrumentExist)
            {
                return Result<CreateMusicListingResponse>.Failure(new Error
                {
                    Code = "Instrument.NotExist",
                    Message = "Instrument does not Exist",
                    Type = ErrorType.NotFound,
                });
            }
            var musicListingSlot = new MusicListingSlot()
            {
                MusicListingId = musicListing.Id,
                InstrumentId = slot.InstrumentId,
                Note = slot.Note,
            };
            musicListing.Slots.Add(musicListingSlot);
        }
        _dbContext.MusicListings.Add(musicListing);
        await _dbContext.SaveChangesAsync();
        
        return Result<CreateMusicListingResponse>.Success(
            new CreateMusicListingResponse
            {
                MusicListingId = musicListing.Id
            });
    }
}
