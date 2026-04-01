using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.GetProfileById;

public class GetProfileByIdHandler
{
    private AppDbContext _dbContext;
    public GetProfileByIdHandler(AppDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public async Task<GetProfileByIdResponse?> HandleGetProfileByIdAsync(GetProfileByIdRequest request)
    {
        IQueryable<AppUser> usersQuery = _dbContext.AppUsers.Where(u => u.Id == request.UserId);

        var query = usersQuery.Select(u => new GetProfileByIdResponse(
            u.Id,
            u.FirstName,
            u.LastName,
            u.Description,
            request.IncludeSocialLinks
                ? u.AppUserLinks.Select(link => new SocialLinkResponse(link.Id, link.Url)).ToList()
                : null
        ));
        var user = await query.FirstOrDefaultAsync();
            
        if (user == null)
            throw new Exception("User not found");

        return user;
    }
    
}