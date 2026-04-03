using GigPH.Domain;
using GigPH.Features.Auth.Login;
using GigPH.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace GigPH.Features.Auth.Register;

public class RegisterHandler
{
    private readonly AppDbContext _dbContext;
    
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    public RegisterHandler(AppDbContext dbcontext, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
        _signInManager = signInManager;
        _dbContext = dbcontext;
        _userManager = userManager;
    }

    public async Task<RegisterResponse> HandleAsync(RegisterRequest request)
    {
        var user = new AppUser()
        {
            UserName = request.Email,
            Email= request.Email,
        };
        var identityResult = await _userManager.CreateAsync(user, request.Password!);
        if (!identityResult.Succeeded)
        {
            throw new Exception(string.Join(", ", identityResult.Errors.Select(e => e.Description)));
        }

        var identityRoleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!identityRoleResult.Succeeded)
        {
            throw new Exception(string.Join(", ", identityRoleResult.Errors));
        }

        return new RegisterResponse()
        {
            Email = request.Email!,
        };

    }
    
    
}


