using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace PrjA.Features.Auth.Login;

public class LoginHandler
{
    private readonly AppDbContext _dbContext;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly RoleManager<IdentityRole> _roleManager;
    public LoginHandler(AppDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JwtOptions> jwtOptions, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtOptions = jwtOptions;
        _roleManager = roleManager;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);

        if (user == null)
        {
            return Result<LoginResponse>.Failure(new Error
            {
                Code = "User.Invalid",
                Message = "Invalida username or password",
                Type = ErrorType.BadRequest,
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            return Result<LoginResponse>.Failure(new Error
            {
                Code = "User.Invalid",
                Message = "Invalida username or password",
                Type = ErrorType.BadRequest,
            });
        }


        var roles = await _userManager.GetRolesAsync(user);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey));

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            .. roles.Select(r => new Claim(ClaimTypes.Role, r))
        ];

        var tokenDescription = new SecurityTokenDescriptor()
        {
            Audience = _jwtOptions.Value.Audience,
            Expires = DateTime.Now.AddMinutes(int.Parse(_jwtOptions.Value.ExpirationInMinutes)),
            Issuer = _jwtOptions.Value.Issuer,
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(claims),
        };
        var accessToken = new JsonWebTokenHandler().CreateToken(tokenDescription);

        if (accessToken == null)
        {
            throw new InvalidOperationException("Failed to create an access token.");
        }

        return Result<LoginResponse>.Success(new LoginResponse()
            {
                AccessToken = accessToken,
                Email = user.Email!,
                UserName = user.UserName!
            });
        
    }
}
