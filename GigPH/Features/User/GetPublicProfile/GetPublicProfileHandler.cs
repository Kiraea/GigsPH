using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.GetProfileById;

public class GetPublicProfileHandler
{
    private AppDbContext _dbContext;
    public GetPublicProfileHandler(AppDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public async Task<GetPublicProfileResponse?> HandleAsync(GetPublicProfileRequest request)
    {
        throw new NotImplementedException();
    }
    
}