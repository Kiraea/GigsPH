using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.User.GetPublicProfile;




[ApiController]
[Route("/api/users")]
public class GetPublicProfileEndpoint: ControllerBase
{
    private readonly GetPublicProfileHandler _handler;
    public GetPublicProfileEndpoint(GetPublicProfileHandler handler)
    {
        _handler = _handler;
    }

    [HttpGet("{userId}/profile")]
    public async Task<ActionResult<GetPublicProfileResponse>> GetPublicProfile(string userId)
    {

        var response = _handler.HandleAsync(userId);

        return Ok(response);
    }
}
