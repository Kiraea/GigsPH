using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.GetPublicBands;

[ApiController]
[AllowAnonymous]
[Route("/api/bands")]
public class GetPublicBandsEndpoint : ControllerBase
{
    private readonly GetPublicBandsHandler _handler;

    public GetPublicBandsEndpoint(GetPublicBandsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<GetPublicBandsResponse>> GetPublicBands(
        [FromQuery] GetPublicBandsRequest request)
    {
        var response = await _handler.HandleAsync(request);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
