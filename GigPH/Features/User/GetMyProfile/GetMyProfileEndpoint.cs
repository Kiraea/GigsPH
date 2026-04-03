using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetProfileById;


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
    public async Task<ActionResult<GetMyProfileResponse>> GetProfile(
        [FromQuery] bool includeSocialLinks = false, [FromQuery] bool includeBands = false)
    {
        
        // the jwtregister.sub thingy automaps to claimtypes.nameidentified for some reason thats why we check that one
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        

        var query = new GetMyProfileRequest()
        {
            UserId = userId,
            IncludeSocialLinks = includeSocialLinks,
            IncludeBands= includeBands,
        };

        var validation = await _validator.ValidateAsync(query);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation.ToString());
        }

        var profile = await _handler.HandleAsync(query);
        return profile!;
    }
}