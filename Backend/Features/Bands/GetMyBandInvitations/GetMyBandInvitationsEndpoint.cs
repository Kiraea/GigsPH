using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.GetMyBandInvitations;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class GetMyBandInvitationsEndpoint : ControllerBase
{
    private readonly GetMyBandInvitationsHandler _handler;

    public GetMyBandInvitationsEndpoint(GetMyBandInvitationsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("invitations")]
    public async Task<ActionResult<List<GetMyBandInvitationResponse>>> GetMyBandInvitations()
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
