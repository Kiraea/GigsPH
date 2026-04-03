using System.Security.Claims;
using GigPH.Features.Post.GetMyPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Post.GetMyPosts;



[ApiController]
[Route("/api/posts")]
public class GetMyPostsEndpoint :ControllerBase
{
    private readonly GetMyPostsHandler _handler;
    public GetMyPostsEndpoint(GetMyPostsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<List<GetMyPostsResponse>>> GetMyPosts()
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        var request = new GetMyPostsRequest
        {
            UserId = result
        };
        var response = await _handler.HandleAsync(request);

        return response;

    }
    
}