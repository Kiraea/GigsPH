using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.Metrics;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.User.UpdateProfile;

public class UpdateProfileHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly  AppDbContext _dbContext;
    public UpdateProfileHandler(UserManager<AppUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<UpdateProfileResponse> Handle(UpdateProfileRequest request, string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);
        
        
        
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (!user.IsOnboarded)
        {
            throw new Exception("Need to be onboarded first");
        }

        await _dbContext.Entry(user).Collection(u => u.Genres).LoadAsync();
        await _dbContext.Entry(user).Collection(u => u.UserInstruments).Query().Include(ui => ui.Instrument).LoadAsync();
        await _dbContext.Entry(user).Collection(u => u.SocialLinks).LoadAsync();
        await _dbContext.Entry(user).Reference(u => u.Location).LoadAsync();
        if (request.DisplayName != null)
        {
            user.DisplayName = request.DisplayName.Trim();
        }
        if (request.Description != null)
        {
            user.Description= request.Description;
        }

        if (request.GenreIds != null)
        {
            var genreIds = request.GenreIds.Distinct().ToList();
            var genres = await _dbContext.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

            if (genres.Count != genreIds.Count)
            {
                throw new Exception("Genre count does not match");
            }

            
            
            user.Genres.Clear();

            foreach (var select in genres)
            {
                user.Genres.Add(select);
            }
            
        }

        if (request.Instruments != null)
        {
            var instrumentsId = request.Instruments.Select(i => i.Id).ToList();
            
            if (instrumentsId.Distinct().Count() != instrumentsId.Count)
            {
                throw new Exception("cannot repeat same instrument");
            }
            
            
            var instruments = await _dbContext.Instruments.Where(i => instrumentsId.Contains(i.Id)).ToListAsync();
            if (instruments.Count != instrumentsId.Count)
            {
                throw new Exception("Instrument count does not match");
            }

            var updatedSelections = new List<UserInstrument>();
            // current piano violin guitar
            // want new piano drums
            
            foreach (var select in request.Instruments)
            {
                var exist = user.UserInstruments.FirstOrDefault(ui =>ui.InstrumentId  == select.Id);
                if (exist == null)
                {
                    exist = new UserInstrument()
                    {
                        AppUserId = user.Id,
                        InstrumentId = select.Id,
                        Instrument = instruments.Single(i => i.Id == select.Id) // this is just so we can use this to return a response
                    };
                }

                exist.isMain = select.IsMain;
                updatedSelections.Add(exist);

            }
            user.UserInstruments = updatedSelections;
        }

        if (request.SocialLinks != null)
        {
            user.SocialLinks.Clear();
            foreach (var select in request.SocialLinks)
            {
                user.SocialLinks.Add(new SocialLink()
                {
                    AppUserId =  user.Id,
                    Platform = select.Platform,
                    Url = select.Url
                });
            }
        }

        if (request.Location != null)
        {
            var country = request.Location.Country.Trim();
            var provinceState = request.Location.ProvinceState.Trim();
            var city = request.Location.City.Trim();
            
            
            var location= await _dbContext.Locations.FirstOrDefaultAsync(l => 
                l.City == city && 
                l.Country == country 
                && l.ProvinceState == provinceState);

            if (location == null)
            {
                
                location = new Location
                {
                    Country = country ,
                    City = city,
                    ProvinceState =  provinceState,
                };
            }

            user.Location = location;
        }
        if (string.IsNullOrWhiteSpace(user.DisplayName))
        {
            throw new Exception("Profile display name is missing.");
        }

        await _dbContext.SaveChangesAsync();

        return new UpdateProfileResponse()
        {
            Id = user.Id,
            DisplayName = user.DisplayName!,
            Description = user.Description,
            Genres = user.Genres.Select(a => new ProfileGenreResponseDto
            {
                Id = a.Id,
                Name = a.Name
            }).ToList(),

            Instruments = user.UserInstruments.Select(ui => new ProfileInstrumentResponseDto
            {
                Id = ui.InstrumentId,
                IsMain = ui.isMain,
                Name = ui.Instrument!.Name
            }).ToList(),
            SocialLinks = user.SocialLinks.Select(s => new ProfileSocialLinkResponseDto
            {
                Platform = s.Platform,
                Url = s.Url,
            }).ToList(),
            Location = new ProfileLocationResponseDto
            {
                Country = user.Location.Country,
                ProvinceState = user.Location.ProvinceState,
                City = user.Location.City,
            },
            IsOnboarded = user.IsOnboarded,
        };

    }
}