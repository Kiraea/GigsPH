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

        var result = await _userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
        {
            string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        var result2 = await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!result2.Succeeded)
        {
            string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        return new RegisterResponse()
        {
            Email = request.Email!
        };
        

    }
    
    
}