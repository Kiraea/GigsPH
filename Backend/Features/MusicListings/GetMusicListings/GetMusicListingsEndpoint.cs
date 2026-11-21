using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.MusicListings.GetMusicListings;

[ApiController]
[AllowAnonymous]
[Route("/api/music-listings")]
public class GetMusicListingsEndpoint : ControllerBase
{
    private readonly GetMusicListingsHandler _handler;
    private readonly IValidator<GetMusicListingsRequest> _validator;

    public GetMusicListingsEndpoint(GetMusicListingsHandler handler, IValidator<GetMusicListingsRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<GetMusicListingsResponse>> GetMusicListings(
        [FromQuery] GetMusicListingsRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(request);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
