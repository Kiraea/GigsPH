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
            .Include(au => au.Bands)
            .Include(au => au.Instruments)
            .Include(au => au.SocialLinks).FirstOrDefaultAsync();

        
        
        if (user == null)
        {
            throw new Exception("user cannot be found");
        }

        user.DisplayName = request.DisplayName!;
        user.Description= request.Description!;
        user.FirstName= request.FirstName!;
        user.LastName= request.LastName!;
        user.IsOnboarded= true;

        if (request.GenreIds.Count > 0)
        {
            var genres = await _dbContext.Genres.Where(g => request.InstrumentIds.Contains(g.Id)).ToListAsync();
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
        };
    }
}