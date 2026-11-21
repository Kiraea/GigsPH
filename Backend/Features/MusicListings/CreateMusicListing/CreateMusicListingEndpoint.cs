using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.CreateMusicListing;

[ApiController]
[Authorize]
[Route("/api/music-listings")]
public class CreateMusicListingEndpoint : ControllerBase
{
    private readonly CreateMusicListingHandler _handler;
    private readonly IValidator<CreateMusicListingRequest> _validator;

    public CreateMusicListingEndpoint(CreateMusicListingHandler handler, IValidator<CreateMusicListingRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateMusicListingResponse>> CreateMusicListing(
        [FromBody] CreateMusicListingRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
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

        var response = await _handler.HandleAsync(request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
