using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.AcceptMusicListingApplicant;

[ApiController]
[Authorize]
[Route("/api/music-listings")]
public class AcceptMusicListingApplicantEndpoint : ControllerBase
{
    private readonly AcceptMusicListingApplicantHandler _handler;

    public AcceptMusicListingApplicantEndpoint(AcceptMusicListingApplicantHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{musicListingId:guid}/applications/{applicationId:guid}/accept")]
    public async Task<ActionResult<AcceptMusicListingApplicantResponse>> AcceptMusicListingApplicant(
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
