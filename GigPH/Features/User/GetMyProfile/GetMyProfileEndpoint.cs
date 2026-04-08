using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetMyProfile;


[ApiController]
[Route("/api/users")]
public class GetMyProfileEndpoint : ControllerBase
{
    private readonly GetMyProfileHandler _handler;
    private readonly IValidator<GetMyProfileRequest> _validator;
    public GetMyProfileEndpoint(GetMyProfileHandler handler, IValidator<GetMyProfileRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<GetMyProfileResponse>> GetMyProfile(
        [FromQuery] GetMyProfileRequest request)
    {
        
        // the jwtregister.sub thingy automaps to claimtypes.nameidentified for some reason thats why we check that one
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        request.UserId = result;

        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation.ToString());
        }

        var profile = await _handler.HandleAsync(request);
        return profile!;
    }
}