using System.Security.Claims;
using FluentValidation;
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
    private readonly IValidator<LeaveBandRequest> _validator;

    public LeaveBandEndpoint(LeaveBandHandler handler, IValidator<LeaveBandRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("leave")]
    public async Task<ActionResult<LeaveBandResponse>> LeaveBand([FromBody] LeaveBandRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
