using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.UpdateProfile;

public class UpdateProfileHandler
{
    private AppDbContext _dbContext;
    public UpdateProfileHandler(AppDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public async Task<UpdateProfileResponse> HandleAsync(Guid requesterUserId, UpdateProfileRequest request)
    {
        var user = await _dbContext.AppUsers.Where(au => au.Id == requesterUserId)
            .Include(au => au.Instruments)
            .Include(au => au.Genres)
            .Include(au => au.Location)
            .Include(au => au.SocialLinks).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new Exception("cannot find user");
        }

        user.Description = request.Description;
        user.Instruments = await _dbContext.Instruments.Where(i => request.InstrumentIds.Contains(i.Id)).ToListAsync();
        user.Genres = await _dbContext.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToListAsync();
        user.SocialLinks.Clear();
        
        
        var location = await _dbContext.Locations.FirstOrDefaultAsync(l =>
            l.Country == request.Country && l.City == request.City && l.ProvinceState == request.ProvinceState);

        if (location == null)
        {
            location = new Location
                { City = request.City!, Country = request.Country!, ProvinceState = request.ProvinceState! };
            await _dbContext.AddAsync(location);
        }

        user.Location = location;


        foreach (var sl in request.SocialLinks)
        {
            user.SocialLinks.Add(new SocialLink { Url = sl });
        }

        await _dbContext.SaveChangesAsync();
        
        return new UpdateProfileResponse() 
        {
            Id = user.Id,
            Description = user.Description,
            SocialLinks = user.SocialLinks
                              ?.Select(aul => new SocialLinkResponse { Id = aul.Id, Url = aul.Url }).ToList()
                          ?? [],
            Genres = user.Genres?.Select(aug => new GenreResponse { Id = aug.Id, Name = aug.Name }).ToList()
                     ?? [],
            Instruments = user.Instruments?.Select(aui => new InstrumentResponse { Id = aui.Id, Name = aui.Name })
                              .ToList()
                          ?? [],
            Location = new LocationResponse{City =location.City, Country = location.Country, ProvinceState = location.ProvinceState, Id = location.Id}
            
        };
    }
    /*
     * var user = await _userManager.FindByIdAsync(result.ToString());
// SocialLinks is empty [] here, EF doesn't know about them
// so user.SocialLinks.Clear() does NOTHING

// you need to go directly to DbContext
var existing = _context.SocialLinks.Where(s => s.AppUserId == result);
_context.SocialLinks.RemoveRange(existing);
     */
        
    }
    
        
       
       
       
       
       
       
       