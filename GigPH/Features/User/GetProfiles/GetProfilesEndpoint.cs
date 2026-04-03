using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetProfileById;


[ApiController]
public class GetProfilesEndpoint : ControllerBase
{

    private readonly GetPublicProfileHandler _handler;
    public GetProfilesEndpoint(GetPublicProfileHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{userId:Guid}")]
    public async Task<ActionResult<GetPublicProfileResponse>> GetProfile(
        [FromRoute] Guid userId,
        [FromQuery] bool includeSocialLinks = false)
    {
        var request = new GetPublicProfileRequest(userId, includeSocialLinks);
        var profile = await _handler.HandleAsync(request);

        return profile;
    }


}