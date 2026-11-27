using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.GetMyBands;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class GetMyBandsEndpoint : ControllerBase
{
    private readonly GetMyBandsHandler _handler;

    public GetMyBandsEndpoint(GetMyBandsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<GetMyBandResponse>>> GetMyBands()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _handler.HandleAsync(userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
