using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Genres;


[ApiController]
[Route("/api")]
public class GetGenresEndpoint: ControllerBase
{

    private readonly GetGenresHandler _handler;
    public GetGenresEndpoint(GetGenresHandler handler)
    {
        _handler = handler;
    }

    
    [HttpGet("genres")]
    public async Task<ActionResult<List<GetGenresResponse>>>GetGenres()
    {
        var response = await _handler.HandleAsync();

        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);

    }
}