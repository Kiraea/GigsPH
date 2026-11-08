using Microsoft.AspNetCore.Identity;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace PrjA.Features.User.Onboarding;

public class OnboardingHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;

    public OnboardingHandler(UserManager<AppUser> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<OnboardingResponse> HandleAsync(OnboardingRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (user.IsOnboarded)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("Onboarding", "Onboarding already completed.");
            });
        }

        var genresId = request.GenreIds.Distinct().ToList();



        
        var genres = await _dbContext.Genres.Where(g=>genresId.Contains(g.Id)).ToListAsync();

        if (genres.Count != request.GenreIds.Count())
        {
            throw new Exception("Some Genres are not found");
        }

        foreach (var genre in genres)
        {
            user.Genres.Add(genre);
        }
        
        
        var instrumentIds = request.Instruments.Select(i => i.InstrumentId).ToArray();
        // THIS IS BASically what exists
        var existingInstruments = await _dbContext.Instruments.Where(i => instrumentIds.Contains(i.Id))
            .ToListAsync();

        if (existingInstruments.Count != instrumentIds.Count())
        {
            throw new ValidationException(
                "One or more selected instruments do not exist.");
        }

        foreach (var instrument in request.Instruments)
        {
            user.UserInstruments.Add(new UserInstrument
            {
                AppUserId = user.Id,
                InstrumentId = instrument.InstrumentId,
                isMain = instrument.IsMain
            });
        }

        
        foreach (var select in request.SocialLinks)
        {
           user.SocialLinks.Add(new SocialLink
           {
               AppUserId = user.Id,
               Platform = select.Platform,
               Url = select.Url,
           }); 
        }
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

        user.IsOnboarded = true;

        
        await _dbContext.SaveChangesAsync();
        return new OnboardingResponse()
        {
            Id = user.Id,
            IsOnboarded = true,
            DisplayName = user.DisplayName
        };
    }
 
}
