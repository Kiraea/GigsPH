using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.User.UpdateProfile;


[ApiController]
[Route("/api/users/")]
public class UpdateProfileEndpoint : ControllerBase
{
    private readonly IValidator<UpdateProfileRequest> _validator;
    private readonly UpdateProfileHandler _handler;
    public UpdateProfileEndpoint(UpdateProfileHandler handler, IValidator<UpdateProfileRequest> validator)
    {
        _validator = validator;
        _handler = handler;
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile( [FromBody] UpdateProfileRequest request)
    {
        var result = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(result, out var requesterUserId))
        {
            return Unauthorized();
        }


        var validationResponse = await _validator.ValidateAsync(request);
        if (!validationResponse.IsValid)
        {
            return ValidationProblem(validationResponse.ToString());
        }

        var response = await _handler.HandleAsync(requesterUserId, request);

        return response;







    }
}