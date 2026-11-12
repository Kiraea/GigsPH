using Microsoft.AspNetCore.Identity;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Auth.Register;

public class RegisterHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    public RegisterHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<RegisterResponse> HandleAsync(RegisterRequest request)
    {

        var user = new AppUser()
        {
            UserName = request.Email,
            Email = request.Email,
        };

        await using var transaction =  await _dbContext.Database.BeginTransactionAsync();
        var result = await _userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
        {
            throw new RegistrationRejectedException(result.Errors);
        }

        var result2 = await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!result2.Succeeded)
        {
            string errors = string.Join(Environment.NewLine, result2.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign the default user role: {errors}");
        }

        await transaction.CommitAsync();
        
        

        return new RegisterResponse()
        {
            Email = request.Email!
        };
        

    }
    
    
}
