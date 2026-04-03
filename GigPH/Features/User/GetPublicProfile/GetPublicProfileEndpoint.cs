using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetProfileById;


[ApiController]
[Route("/api/users")]
public class GetPublicProfileEndpoint : ControllerBase
{
    private readonly GetPublicProfileHandler _handler;
    public GetPublicProfileEndpoint(GetPublicProfileHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{userId:Guid}")]
    public async Task<ActionResult<GetPublicProfileResponse>> GetProfile(
        [FromRoute] Guid userId,
        [FromQuery] bool includeSocialLinks = false)
    {
        var query = new GetPublicProfileRequest(userId, includeSocialLinks);
        var profile = await _handler.HandleAsync(query);
        return profile;
    }
    


}