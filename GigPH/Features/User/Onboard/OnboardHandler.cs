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
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync((au) => au.Id == request.UserId);

        if (user == null)
        {
            throw new Exception("user cannot be found");
        }

        user.DisplayName = request.DisplayName!;
        user.Description= request.Description!;
        user.FirstName= request.FirstName!;
        user.LastName= request.LastName!;
        user.IsOnboarded= true;

        //_dbContext.Update(user);
        // if ur fetching the user technically no need to like do this

        await _dbContext.SaveChangesAsync();

        return new OnboardResponse()
        {
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Description = user.Description,
        };
    }
}