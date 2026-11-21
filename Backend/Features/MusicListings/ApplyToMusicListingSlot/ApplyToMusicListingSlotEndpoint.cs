using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.ApplyToMusicListingSlot;

[ApiController]
[Authorize]
[Route("/api/music-listings")]
public class ApplyToMusicListingSlotEndpoint : ControllerBase
{
    private readonly ApplyToMusicListingSlotHandler _handler;
    private readonly IValidator<ApplyToMusicListingSlotRequest> _validator;

    public ApplyToMusicListingSlotEndpoint(ApplyToMusicListingSlotHandler handler, IValidator<ApplyToMusicListingSlotRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("{musicListingId:guid}/slots/{slotId:guid}/applications")]
    public async Task<ActionResult<ApplyToMusicListingSlotResponse>> ApplyToMusicListingSlot(
        [FromRoute] Guid musicListingId,
        [FromRoute] Guid slotId,
        [FromBody] ApplyToMusicListingSlotRequest request)
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

        if (slotId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(slotId), "Slot ID must not be empty.");
        }

        var result = await _validator.ValidateAsync(request);
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(musicListingId, slotId, request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
