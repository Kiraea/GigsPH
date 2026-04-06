using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.GetProfileById;

public class GetMyProfileHandler
{
    private AppDbContext _dbContext;
    public GetMyProfileHandler(AppDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public async Task<GetMyProfileResponse> HandleAsync(GetMyProfileRequest request)
    {
        // as no trakcing since we dont really change aynthing we get straightup
        var userQuery = _dbContext.AppUsers.AsNoTracking().Where(au => au.Id == request.UserId);
        if (request.IncludeBands)
        {
            userQuery = userQuery.Include(au => au.Bands);
        }

        if (request.IncludeGenres)
        {
            
            userQuery = userQuery.Include(au => au.Genres);
        }

        if (request.IncludeInstruments)
        {
            
            userQuery = userQuery.Include(au => au.Instruments);
        }

        if (request.IncludeSocialLinks)
        {
            userQuery = userQuery.Include(au => au.SocialLinks);
        }
        var user = await userQuery.FirstOrDefaultAsync();
        
        if (user == null)
        {
            throw new Exception("user does not exist");
        }

        var userResponse = new GetMyProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Description = user.Description,
            SocialLinks = user.SocialLinks
                              ?.Select(aul => new SocialLinkResponse { Id = aul.Id, Url = aul.Url }).ToList()
                          ?? [],
            Bands = user.Bands?.Select(aub => new BandResponse
                        { Id = aub.Id, Description = aub.Description, Title = aub.Title }).ToList()
                    ?? [],
            Genres = user.Genres?.Select(aug => new GenreResponse { Id = aug.Id, Name = aug.Name }).ToList()
                     ?? [],
            Instruments = user.Instruments?.Select(aui => new InstrumentResponse { Id = aui.Id, Name = aui.Name })
                              .ToList()
                          ?? []
        };

        return userResponse;
    }
    
}