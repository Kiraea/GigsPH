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

        var user = await _dbContext.AppUsers.Where(au => au.Id == request.UserId)
            .Include(au => au.AppUserLinks)
            .Include(au => au.BandUsers).ThenInclude(bandUser => bandUser.Band)
            .FirstOrDefaultAsync();
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
            SocialLinks = request.IncludeSocialLinks
                ? user.AppUserLinks.Select(aul => new SocialLinkResponse { SocialLinkId = aul.Id, Url = aul.Url }).ToList()
                : new List<SocialLinkResponse>(),
            BandResponses = request.IncludeBands
                ? user.BandUsers.Select(aub => new BandResponse { BandId = aub.BandId, Description = aub.Band.description, Title = aub.Band.title }).ToList()
                : new List<BandResponse>()
        };;


            
        if (userResponse == null)
            throw new Exception("User not found");

        return userResponse;
    }
    
}