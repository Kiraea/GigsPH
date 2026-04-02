using GigPH.Domain;
using GigPH.Features.User.Shared.Dto;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.User.UpdateProfile;

public class UpdateProfileHandler
{
    private AppDbContext _dbContext;
    public UpdateProfileHandler(AppDbContext dbContext)
    {
        _dbContext= dbContext;
    }

    public async Task<UpdateProfileResponse?> Handle(UpdateProfileRequest request)
    {
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(au => au.Id == request.UserId);
        if (user == null)
        {
            throw new Exception("cannot find user");
        }

        return null;


    }
    
}