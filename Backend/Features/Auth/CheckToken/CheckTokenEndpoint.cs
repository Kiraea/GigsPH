using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Auth.CheckToken;



[ApiController]
[Route("/api/checkToken")]
public class CheckTokenEndpoint : ControllerBase
{

    private readonly CheckTokenHandler _handler;
    public CheckTokenEndpoint(CheckTokenHandler handler)
    {
        _handler  = handler;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<CheckTokenResponse>> CheckToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Session.Invalid",
                Detail = "The session does not identify a user."
            });
        }

        var response = await _handler.HandleAsync(userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }


}


public class CheckTokenHandler
{

    private readonly UserManager<AppUser> _userManager;
    public CheckTokenHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<CheckTokenResponse>> HandleAsync(string userId)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {

            return Result<CheckTokenResponse>.Failure(new Error
            {
                Code = "Session.Invalid",
                Message = "The account for this session was not found.",
                Type = ErrorType.Unauthorized
            });
        }

        return Result<CheckTokenResponse>.Success(new CheckTokenResponse
            {
                DisplayName = user.DisplayName,
                Id = user.Id,
                IsOnboarded = user.IsOnboarded,
            });

    }
}
