using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Post.GetPublicPosts;



[ApiController]
[Route("api/posts")]
public class GetPublicPostsEndpoint : ControllerBase 
{
    private readonly GetPublicPostsHandler _handler;

    // 2. Inject the specific handler for this feature
    public GetPublicPostsEndpoint(GetPublicPostsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<GetPublicPostsWrapperResponse>> GetPublicPosts(
        [FromQuery] int page, [FromQuery] int limit)
    {
        var response = await _handler.HandleAsync(page, limit);
        return response;
    }
}