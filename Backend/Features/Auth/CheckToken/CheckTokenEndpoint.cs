using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using PrjA.Domain.Entities;

namespace PrjA.Features.Auth.CheckToken;



[ApiController]
[Route("/api/checkToken")]
public class CheckTokenEndpoint : ControllerBase
{
    
    private readonly CheckTokenHandler _handler;
    public CheckTokenEndpoint(CheckTokenHandler handler)
    {
        _handler  = handler;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<CheckTokenResponse>> CheckToken()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await _handler.Handle(userId);

        return response;
    }


}


public class CheckTokenHandler
{
    
    private readonly UserManager<AppUser> _userManager;
    public CheckTokenHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CheckTokenResponse> Handle(string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new CheckTokenResponse()
        {
            DisplayName = user.DisplayName,
            Id = user.Id,
            IsOnboarded = user.IsOnboarded,
        };

    }
}