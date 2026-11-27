using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Features.Genres;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.GetMusicListings;

public class GetMusicListingsHandler
{
    private readonly AppDbContext _dbContext;

    public GetMusicListingsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetMusicListingsResponse>> HandleAsync(GetMusicListingsRequest request)
    {

        // request page 2 
        // request pagesize 10 items
        
        // reqeusst page 1 
        // request pagesize 10 items
        var skip = (request.Page - 1) * request.PageSize;

        var query = _dbContext.MusicListings.AsNoTracking();

        if (request.GenreId.HasValue)
        {
            query = query.Where(ml => ml.GenreId == request.GenreId.Value);
        }

        if (request.InstrumentId.HasValue)
        {
            query = query.Where(ml => ml.Slots.Any(s => s.InstrumentId == request.InstrumentId.Value && !s.Applications.Any(a => a.Status == MusicListingApplicationStatus.Accepted)));
        }

        if (request.City != null)
        {
            query = query.Where(ml => ml.City == request.City);
        }

        if (request.StartsAfter != null)
        {
            query = query.Where(ml => ml.StartsAt >= request.StartsAfter.Value);
        }
        
        
        // p1 p2 p3 p4 p5 p6 p7 p8 p9 p10 p11 p12 p13 p14 p15
        var listOfMusiclistings = await query.OrderByDescending(ml => ml.CreatedAt)
            .ThenBy(ml => ml.Id)
            .Select(ml => new MusicListingDto()
        {
            Id = ml.Id,
            Title = ml.Title,
            Description = ml.Description,
            EndsAt = ml.EndsAt,
            StartsAt = ml.StartsAt,
            ExpiresAt = ml.ExpiresAt,
            Genre = ml.Genre == null ? null : new GenreDto()
            {
                Id = ml.Genre.Id,
                Name = ml.Genre.Name
            },
            City =  ml.City,
            Slots = ml.Slots.Select(mls => new MusicListingSlotDto()
            {
                Id = mls.Id,
                InstrumentId = mls.InstrumentId,
                IsFilled = mls.Applications.Any(mla => mla.Status == MusicListingApplicationStatus.Accepted),
                Note = mls.Note,
                
            }).ToList(),
        }).Skip(skip).Take(request.PageSize).ToListAsync();
        


        return Result<GetMusicListingsResponse>.Success(new GetMusicListingsResponse()
        {
            Posts = listOfMusiclistings,
        });



    }
}
