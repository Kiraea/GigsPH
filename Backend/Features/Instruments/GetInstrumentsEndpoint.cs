using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.Instruments;

[ApiController]
[Route("/api")]
public class GetInstrumentsEndpoint : ControllerBase
{
    private readonly GetInstrumentsHandler _handler;

    public GetInstrumentsEndpoint(GetInstrumentsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("instruments")]
    public async Task<ActionResult<List<GetInstrumentsResponse>>> GetInstruments()
    {
        var response = await _handler.HandleAsync();

        return response;
    }
}
