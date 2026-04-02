using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetProfileById;


[ApiController]
public class GetProfilesEndpoint : ControllerBase
{

    private readonly GetProfileByIdHandler _handler;
    public GetProfilesEndpoint(GetProfileByIdHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{userId:Guid}")]
    public async Task<ActionResult<GetProfileByIdResponse>> GetProfile(
        [FromRoute] Guid userId,
        [FromQuery] bool includeSocialLinks = false)
    {
        var request = new GetProfileByIdRequest(userId, includeSocialLinks);
        var profile = await _handler.HandleAsync(request);

        return profile;
    }


}