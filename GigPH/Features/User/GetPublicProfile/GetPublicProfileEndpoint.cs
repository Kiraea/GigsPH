using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetPublicProfile;


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
        [FromRoute] Guid userId )
    {


        var profile = await _handler.HandleAsync(userId);
        return profile;
    }
    


}