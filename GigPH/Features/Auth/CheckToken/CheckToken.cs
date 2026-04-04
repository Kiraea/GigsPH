using System.Security.Claims;
using GigPH.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.Auth.CheckToken;

public record CheckTokenResponse
{
    public Guid UserId { get; init; }
    public String? DisplayName { get; init; }
    public bool IsOnboarded { get; init; }
}


[ApiController]
[Route("/api/auth/check-token")]
public class CheckTokenEndpoint :ControllerBase
{

    private readonly CheckTokenHandler _handler;
    public CheckTokenEndpoint(CheckTokenHandler handler)
    {
        _handler = handler;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<CheckTokenResponse>> CheckToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        var response = await _handler.HandleAsync(result);

        return response;

    }
    
    
}


public class CheckTokenHandler
{
    private readonly AppDbContext _dbContext;
    public CheckTokenHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckTokenResponse> HandleAsync(Guid userId)
    {
        var user = _dbContext.AppUsers.Where(au => au.Id == userId)
            .Select(au => new CheckTokenResponse()
            {
                DisplayName = au.DisplayName,
                IsOnboarded = au.IsOnboarded,
                UserId = au.Id,
            });

        var response = await user.FirstOrDefaultAsync();
        if (response == null)
        {
            throw new Exception("cannot find user");
        }
        return response;

    }
}
