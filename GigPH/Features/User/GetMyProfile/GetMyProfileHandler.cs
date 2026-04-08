using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.GetMyProfile;

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
        var user= await _dbContext.AppUsers.AsNoTracking().Where(au => au.Id == request.UserId)
            .Include(au => au.Genres)
            .Include(au => au.Instruments)
            .Include(au => au.Location)
            .Include(au => au.SocialLinks).FirstOrDefaultAsync();
        
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

            Genres = user.Genres?.Select(aug => new GenreResponse { Id = aug.Id, Name = aug.Name }).ToList()
                     ?? [],
            Instruments = user.Instruments?.Select(aui => new InstrumentResponse { Id = aui.Id, Name = aui.Name })
                              .ToList()
                          ?? [],
            Location = user.LocationId != null ? new LocationResponse{City = user.Location.City, Country = user.Location.Country, Id = user.LocationId.Value, ProvinceState = user.Location.ProvinceState} :null 
        };

        return userResponse;
    }
    
}