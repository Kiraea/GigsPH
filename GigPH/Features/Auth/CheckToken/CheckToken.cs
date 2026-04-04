using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Auth.CheckToken;

public record CheckTokenResponse
{
    public Guid UserId { get; init; }
}


[ApiController]
[Route("/api/auth/check-token")]
public class CheckTokenEndpoint :ControllerBase
{
    public CheckTokenEndpoint()
    {
           
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<CheckTokenResponse>> CheckToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        return new CheckTokenResponse
        {
            UserId = result
        };
    }
    
    
}
