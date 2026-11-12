using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidSessionException();
            }

            var response = await _handler.HandleAsync(userId);
            return Ok(response);
        }
        catch (InvalidSessionException exception)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = exception.Message
            });
        }
    }


}


public class CheckTokenHandler
{
    
    private readonly UserManager<AppUser> _userManager;
    public CheckTokenHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CheckTokenResponse> HandleAsync(string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidSessionException();
        }

        return new CheckTokenResponse()
        {
            DisplayName = user.DisplayName,
            Id = user.Id,
            IsOnboarded = user.IsOnboarded,
        };

    }
}
