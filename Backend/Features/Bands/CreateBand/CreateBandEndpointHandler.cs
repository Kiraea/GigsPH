using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.CreateBand;

public class CreateBandEndpointHandler
{

    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    public CreateBandEndpointHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;

    }

    public async Task<Result<CreateBandResponse>> HandleAsync(CreateBandRequest request, string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<CreateBandResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });

        }

        //await _dbContext.Entry(user).Collection((u) => u.UserBands).LoadAsync();
        // dont need cause u can just Add even if i dont loa the colleciton

        var bandExist = _dbContext.Bands.Any(b => b.Name == request.Name);

        if (bandExist)
        {
            return Result<CreateBandResponse>.Failure(new Error
            {
                Code = "Band.AlreadyExists",
                Message = "Band with the same name already exists",
                Type = ErrorType.BadRequest,
            });

        }

        var band = new Band()
        {
            Name = request.Name,
            Description = request.Description ,
        };

        _dbContext.Add(band);

        var userBand = new UserBand()
        {
            Band = band,
            Role = BandRole.Leader,
            UserId = userId
        };

        user.UserBands.Add(userBand);
        
        await _dbContext.SaveChangesAsync();

        return Result<CreateBandResponse>.Success(new CreateBandResponse()
        {
            Id = band.Id,
            Name = band.Name,
            description = band.Description
        });











    }
}