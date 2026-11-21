using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.GetMusicListingApplications;

[ApiController]
[Authorize]
[Route("/api/music-listings")]
public class GetMusicListingApplicationsEndpoint : ControllerBase
{
    private readonly GetMusicListingApplicationsHandler _handler;

    public GetMusicListingApplicationsEndpoint(GetMusicListingApplicationsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{musicListingId:guid}/applications")]
    public async Task<ActionResult<GetMusicListingApplicationsResponse>> GetMusicListingApplications(
        [FromRoute] Guid musicListingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (musicListingId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(musicListingId), "Music listing ID must not be empty.");
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(musicListingId, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
