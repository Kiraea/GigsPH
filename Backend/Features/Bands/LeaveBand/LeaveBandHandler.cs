using Microsoft.AspNetCore.Identity;
using PrjA.Common;
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

    public async Task<Result<LeaveBandResponse>> HandleAsync(LeaveBandRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<LeaveBandResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        await _dbContext.Entry(user).Collection((u) => u.UserBands).LoadAsync();
        var userBand = user.UserBands.FirstOrDefault(ub => ub.BandId ==  request.Id);
        if (userBand == null)
        {
            return Result<LeaveBandResponse>.Failure(new Error
            {
                Code = "Band.NotMember",
                Message = "You are not a member of this band.",
                Type = ErrorType.BadRequest,
            });
        }
        
        user.UserBands.Remove(userBand);
        await _dbContext.SaveChangesAsync();

        return Result<LeaveBandResponse>.Success(new LeaveBandResponse()
        {
            Id = userBand.BandId,
        });
        
        


    }
}
