using System.Security.Claims;
using System.Text;
using GigPH.Domain;
using GigPH.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace GigPH.Features.Auth.Login;

public class LoginHandler
{
    private readonly AppDbContext _dbContext;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppUser> _roleManager;
    private readonly UserManager<AppUser>  _userManager;
    private readonly IOptions<JwtOptions>  _options;
    public LoginHandler(IOptions<JwtOptions> options, AppDbContext dbContext, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<AppUser> roleManager)
    {
        _dbContext = dbContext;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userManager = userManager;
        _options = options;
    }

    public async Task<LoginResponse> HandleAsync(LoginRequest request)
    {
        var user = await  _userManager.FindByEmailAsync(request.Username);
        if (user == null)
        {
            throw new Exception("user not found");
        }

        var signInResponse = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!signInResponse.Succeeded)
        {
            List<string> errors = [];
            if (signInResponse.IsLockedOut)
            {
               errors.Add("is locked out"); 
            }

            if (signInResponse.IsNotAllowed)
            {
                errors.Add("is not allowed");
            }

            if (signInResponse.RequiresTwoFactor)
            {
                errors.Add("needs two factor");
            }

            throw new Exception(string.Join(", ", errors));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SigningKey));
        
        // this is not signing the token this i sjust config
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.Sha256);

        List<Claim> claims = 
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            ..roles.Select(role => new Claim(ClaimTypes.Role, role)),
        ];

        var tokenDescription = new SecurityTokenDescriptor()
        {
            Audience = _options.Value.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_options.Value.ExpirationInMinutes),
            Issuer = _options.Value.Issuer,
            SigningCredentials = credentials,
        };

        var accessToken = new JsonWebTokenHandler().CreateToken(tokenDescription);

        return new LoginResponse()
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
        };
        
        
        




    }
    
    
    
}