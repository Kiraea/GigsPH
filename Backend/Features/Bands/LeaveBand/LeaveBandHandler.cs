using Microsoft.AspNetCore.Identity;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.LeaveBand;

public class LeaveBandHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public LeaveBandHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<LeaveBandResponse> HandleAsync(LeaveBandRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        await _dbContext.Entry(user).Collection((u) => u.UserBands).LoadAsync();
        var userBand = user.UserBands.FirstOrDefault(ub => ub.BandId ==  request.Id);
        if (userBand == null)
        {
            throw new Exception("User is not in this Band ");
        }
        
        user.UserBands.Remove(userBand);
        await _dbContext.SaveChangesAsync();

        return new LeaveBandResponse()
        {
            Id = userBand.BandId,
        };
        
        


    }
}