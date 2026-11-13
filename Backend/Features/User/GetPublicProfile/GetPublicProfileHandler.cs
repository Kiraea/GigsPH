using Microsoft.EntityFrameworkCore;
using PrjA.Features.User.UpdateProfile;
using PrjA.Infrastructure;
using PrjA.Common;

namespace PrjA.Features.User.GetPublicProfile;

public class GetPublicProfileHandler
{
    private readonly AppDbContext _dbContext;
    public GetPublicProfileHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;

    }

    public async Task<Result<GetPublicProfileResponse>> HandleAsync(string userId)
    {
        
        var user = await _dbContext.Users.AsNoTracking().Include(u => u.Genres)
            .Include(u => u.UserInstruments).ThenInclude(ui => ui.Instrument)
            .Include(u => u.SocialLinks)
            .Include(u => u.Location)
            .FirstOrDefaultAsync(au => au.Id == userId && au.IsOnboarded == true);

        if (user == null)
        {
            return Result<GetPublicProfileResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "Profile not found.",
                Type = ErrorType.NotFound,
            });
        }

        return Result<GetPublicProfileResponse>.Success(new GetPublicProfileResponse
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
        });

    }
    
}