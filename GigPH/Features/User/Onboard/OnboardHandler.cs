using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.Onboard;

public class OnboardHandler
{
    private readonly AppDbContext _dbContext;
    public OnboardHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<OnboardResponse> HandleAsync(OnboardRequest request)
    {
        var user = await _dbContext.AppUsers.Where((au) => au.Id == request.UserId)
            .Include(au => au.Instruments)
            .Include(au => au.SocialLinks)
            .Include(au => au.Genres)
            .Include(au => au.Location).FirstOrDefaultAsync();

        
        
        if (user == null)
        {
            throw new Exception("user cannot be found");
        }

        if (user.IsOnboarded)
            throw new Exception("user already onboarded");
        
        user.DisplayName = request.DisplayName!;
        user.Description= request.Description!;
        user.FirstName= request.FirstName!;
        user.LastName= request.LastName!;


        var location = await _dbContext.Locations.FirstOrDefaultAsync(l =>
            l.Country == request.Country && l.City == request.City && l.ProvinceState == request.ProvinceState);

        if (location == null)
        {
            location = new Location
                { City = request.City!, Country = request.Country!, ProvinceState = request.ProvinceState! };
            await _dbContext.AddAsync(location);
        }

        user.Location = location;
        
        

        if (request.GenreIds.Count > 0)
        {
            var genres = await _dbContext.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToListAsync();
            user.Genres = genres;
        }

        if (request.InstrumentIds.Count > 0)
        {
            var instruments =
                await _dbContext.Instruments.Where(i => request.InstrumentIds.Contains(i.Id)).ToListAsync();
            user.Instruments = instruments;
        }

        if (request.SocialLinks.Count > 0)
        {
            foreach (var sl in request.SocialLinks)
            {
                user.SocialLinks.Add(new SocialLink {Url = sl.Url });
            }
        }

        
        user.IsOnboarded= true;
        //_dbContext.Update(user);
        // if ur fetching the user technically no need to like do this

        await _dbContext.SaveChangesAsync();
        return new OnboardResponse() {
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Description = user.Description,
            // in the response the [] is ignored if u explicit set so u still have to do ?? []
            Genres = user.Genres?.Select((g) => new GenreResponse(){Id=g.Id,Name = g.Name}).ToList() ?? [],
            Instruments = user.Instruments?.Select((i) => new InstrumentResponse{Id=i.Id,Name = i.Name}).ToList() ?? [],
            SocialLinks= user.SocialLinks?.Select((sl) => new SocialLinkResponse{Id=sl.Id, Url = sl.Url}).ToList() ?? [],
            Location = new LocationResponse{City = user.Location.City, Country = user.Location.Country, Id = user.Location.Id, ProvinceState = user.Location.ProvinceState}
        };
    }
}