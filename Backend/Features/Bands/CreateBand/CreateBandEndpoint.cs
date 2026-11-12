using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.Bands.CreateBand;


[ApiController]
[Route("/api/")]
public class CreateBandEndpoint : ControllerBase
{
    private readonly CreateBandEndpointHandler _handler;
    private readonly IValidator<CreateBandRequest> _validator;
    public CreateBandEndpoint(CreateBandEndpointHandler handler, IValidator<CreateBandRequest> validator)
    {
        _handler = handler;
        _validator =  validator;
    }

    [HttpPost("create")]

    public async Task<ActionResult<CreateBandResponse>> CreateBand(CreateBandRequest request)
    {
        
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
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
        
        return Ok(response);
        
    }
    
}