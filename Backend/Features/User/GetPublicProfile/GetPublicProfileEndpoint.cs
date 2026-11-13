using Microsoft.AspNetCore.Mvc;

using PrjA.Infrastructure;

namespace PrjA.Features.User.GetPublicProfile;




[ApiController]
[Route("/api/users")]
public class GetPublicProfileEndpoint: ControllerBase
{
    private readonly GetPublicProfileHandler _handler;
    public GetPublicProfileEndpoint(GetPublicProfileHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{userId}/profile")]
    public async Task<ActionResult<GetPublicProfileResponse>> GetPublicProfile(string userId)
    {

        var response = await _handler.HandleAsync(userId);

        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
