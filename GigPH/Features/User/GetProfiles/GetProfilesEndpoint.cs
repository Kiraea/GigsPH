using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetProfileById;


[ApiController]
public class GetProfilesEndpoint : ControllerBase
{

    public GetProfilesEndpoint()
    {
        
    }

    [HttpGet("{userId:Guid}")]
    public Task<ActionResult<GetProfileByIdResponse>> GetProfile(
        [FromRoute] Guid userId,
        [FromQuery] bool includeSocialLinks = false)
    {
        var profile = new (userId, includeSocialLinks);

        return profile;

    }


}