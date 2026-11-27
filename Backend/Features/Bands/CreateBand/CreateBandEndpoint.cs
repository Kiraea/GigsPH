using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.CreateBand;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class CreateBandEndpoint : ControllerBase
{
    private readonly CreateBandEndpointHandler _handler;
    private readonly IValidator<CreateBandRequest> _validator;
    public CreateBandEndpoint(CreateBandEndpointHandler handler, IValidator<CreateBandRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateBandResponse>> CreateBand([FromBody] CreateBandRequest request)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        
        var valid = await _validator.ValidateAsync(request);
        if (!valid.IsValid)
        {
            foreach (var errors in valid.Errors)
            {
                ModelState.AddModelError(errors.PropertyName, errors.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }
        
        var response = await _handler.HandleAsync(request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
