using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PrjA.Common;
using PrjA.Infrastructure;

namespace PrjA.Features.Auth.Login;

[ApiController]
[Route("/api/auth")]
public class LoginEndpoint : ControllerBase
{
    private readonly IValidator<LoginRequest> _validator;
    private readonly LoginHandler _handler;
    private readonly IWebHostEnvironment _env;
    
    public LoginEndpoint(LoginHandler handler, IValidator<LoginRequest> validator, IWebHostEnvironment env)
    {
        _validator = validator;
        _handler = handler;
        _env = env;

    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    error.PropertyName,
                    error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }


        var response = await _handler.HandleAsync(request);
        if (!response.isSuccess)
        {
            return response.ToProblemDetails(this);
        }

        Response.Cookies.Append("access_token", response.Value!.AccessToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
        });

        return Ok(response.Value);
        


    }
    
    
}
