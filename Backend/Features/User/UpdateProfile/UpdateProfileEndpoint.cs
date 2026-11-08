using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Features.User.Onboarding;

namespace PrjA.Features.User.UpdateProfile;




[ApiController]
[Route("/api/user")]
public class UpdateProfileEndpoint : ControllerBase
{
    private readonly IValidator<UpdateProfileRequest> _validator;
    private readonly UpdateProfileHandler _handler;
    public UpdateProfileEndpoint(IValidator<UpdateProfileRequest> validator, UpdateProfileHandler handler)
    {

        _validator = validator;
        _handler = handler;
    }

    [HttpPost("update")]
    [Authorize]
    public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile(UpdateProfileRequest request)
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var validateResult = await _validator.ValidateAsync(request);
        if (!validateResult.IsValid)
        {
            
            string errors = string.Join(Environment.NewLine, validateResult.Errors.Select(e => e.ErrorMessage));
            return BadRequest(errors);
        }
        
        var response = _handler.HandleAsync(request, userId);
        
    }
}
