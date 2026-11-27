using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.LeaveBand;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class LeaveBandEndpoint : ControllerBase
{
    private readonly LeaveBandHandler _handler;

    public LeaveBandEndpoint(LeaveBandHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{bandId:guid}/leave")]
    public async Task<ActionResult<LeaveBandResponse>> LeaveBand([FromRoute] Guid bandId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (bandId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(bandId), "Band ID must not be empty.");
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(bandId, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
