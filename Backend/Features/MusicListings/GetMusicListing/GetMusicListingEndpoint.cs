using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.GetMusicListing;

[ApiController]
[AllowAnonymous]
[Route("/api/music-listings")]
public class GetMusicListingEndpoint : ControllerBase
{
    private readonly GetMusicListingHandler _handler;

    public GetMusicListingEndpoint(GetMusicListingHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{musicListingId:guid}")]
    public async Task<ActionResult<MusicListingDto>> GetMusicListing(
        [FromRoute] Guid musicListingId)
    {
        if (musicListingId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(musicListingId), "Music listing ID must not be empty.");
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(musicListingId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
