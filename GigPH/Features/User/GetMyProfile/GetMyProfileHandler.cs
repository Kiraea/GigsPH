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

    public async Task<GetMyProfileResponse?> HandleAsync(GetMyProfileRequest request)
    {

        var user = await _dbContext.AppUsers.Where(au => au.Id == request.UserId)
            .Select(au => new GetMyProfileResponse()
            {
                Id = au.Id,
                FirstName = au.FirstName,
                LastName = au.LastName,
                Description = au.Description,
                SocialLinks = request.IncludeSocialLinks
                    ? au.AppUserLinks.Select(aul => new SocialLinkResponse { SocialLinkId = aul.Id, Url = aul.Url })
                        .ToList()
                    : new List<SocialLinkResponse>(),
                BandResponses = request.IncludeBands
                    ? au.BandUsers.Select(aub => new BandResponse
                        { BandId = aub.BandId, Description = aub.Band.description, Title = aub.Band.title }).ToList()
                    : new List<BandResponse>()
            }).FirstOrDefaultAsync();


            
        if (user == null)
            throw new Exception("User not found");

        return user;
    }
    
}