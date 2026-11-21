using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.CancelMusicListingApplication;

[ApiController]
[Authorize]
[Route("/api/music-listings")]
public class CancelMusicListingApplicationEndpoint : ControllerBase
{
    private readonly CancelMusicListingApplicationHandler _handler;

    public CancelMusicListingApplicationEndpoint(CancelMusicListingApplicationHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{musicListingId:guid}/applications/{applicationId:guid}/cancel")]
    public async Task<ActionResult<CancelMusicListingApplicationResponse>> CancelMusicListingApplication(
        [FromRoute] Guid musicListingId,
        [FromRoute] Guid applicationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (musicListingId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(musicListingId), "Music listing ID must not be empty.");
        }

        if (applicationId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(applicationId), "Application ID must not be empty.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(musicListingId, applicationId, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
