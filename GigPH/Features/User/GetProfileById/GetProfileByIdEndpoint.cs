using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigPH.Features.User.GetProfileById;


[ApiController]
[Route("/api/users")]
public class GetProfileByIdEndpoint : ControllerBase
{
    private readonly GetProfileByIdHandler _handler;
    public GetProfileByIdEndpoint(GetProfileByIdHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{userId:Guid}")]
    public async Task<ActionResult<GetProfileByIdResponse>> GetProfile(
        [FromRoute] Guid userId,
        [FromQuery] bool includeSocialLinks = false)
    {
        var query = new GetProfileByIdRequest(userId, includeSocialLinks);
        var profile = await _handler.HandleAsync(query);
        return profile;
    }
    


}