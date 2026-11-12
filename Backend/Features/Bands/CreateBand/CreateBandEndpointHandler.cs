using Microsoft.AspNetCore.Identity;
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

    public async Task<CreateBandResponse> HandleAsync(CreateBandRequest request, string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("user does not exist");
        }

        //await _dbContext.Entry(user).Collection((u) => u.UserBands).LoadAsync();
        // dont need cause u can just Add even if i dont loa the colleciton

        var bandExist = _dbContext.Bands.Any(b => b.Name == request.Name);

        if (bandExist)
        {
            throw new Exception("band name already exists");
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

        return new CreateBandResponse()
        {
            Id = band.Id,
            Name = band.Name,
            description = band.Description
        };
        










    }
}