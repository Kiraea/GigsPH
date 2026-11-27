using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.User.GetUsers;

[ApiController]
[AllowAnonymous]
[Route("/api/users")]
public class GetUsersEndpoint : ControllerBase
{
    private readonly GetUsersHandler _handler;

    public GetUsersEndpoint(GetUsersHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers(
        [FromQuery] string? search, [FromQuery] int page = 1, int  pageSize = 10)
    {
        var response = await _handler.HandleAsync(search, page, pageSize);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
