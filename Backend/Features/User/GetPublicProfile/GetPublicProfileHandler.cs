using Microsoft.EntityFrameworkCore;
using PrjA.Features.User.UpdateProfile;
using PrjA.Infrastructure;

namespace PrjA.Features.User.GetPublicProfile;

public class GetPublicProfileHandler
{
    private readonly AppDbContext _dbContext;
    public GetPublicProfileHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;

    }

    public async Task<GetPublicProfileResponse> HandleAsync(string userId)
    {
        
        var user = await _dbContext.Users.AsNoTracking().Include(u => u.Genres)
            .Include(u => u.UserInstruments).ThenInclude(ui => ui.Instrument)
            .Include(u => u.Bands)
            .Include(u => u.SocialLinks)
            .Include(u => u.Location)
            .FirstOrDefaultAsync(au => au.Id == userId && au.IsOnboarded == true);

        if (user == null)
        {
            throw new Exception("no user found");
        }

        return new GetPublicProfileResponse
        {
            Firstname = user.FirstName!,
            LastName = user.LastName!,
            Description = user.Description!,
            DisplayName = user.DisplayName!,
            Genres = user.Genres.Select(g => new ProfileGenreResponseDto()
            {
                Id = g.Id,
                Name = g.Name,
            }).ToList(),
            Location = new ProfileLocationResponseDto()
            {
                City = user.Location!.City,
                Country = user.Location.Country,
                ProvinceState = user.Location.ProvinceState,
            },
            SocialLinks = user.SocialLinks.Select((s => new ProfileSocialLinkResponseDto
            {
                Platform = s.Platform,
                Url = s.Url,

            })).ToList(),
            UserInstruments = user.UserInstruments.Select(ui => new ProfileInstrumentResponseDto
            {
                IsMain = ui.isMain,
                Name = ui.Instrument!.Name,
                Id = ui.Instrument.Id,
            }).ToList()
        };

    }
    
}