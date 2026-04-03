using System.Collections.Immutable;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Auth.Login;



[ApiController]
[Route("/api/auth")]
public class LoginEndpoint : ControllerBase
{

    private readonly LoginHandler _handler;
    private readonly IValidator<LoginRequest> _validator;
    private readonly IWebHostEnvironment _env;
    public LoginEndpoint(LoginHandler handler, IValidator<LoginRequest> validator, IWebHostEnvironment env)
    {
        _handler = handler;
        _validator = validator;
        _env = env;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var validationResults = await  _validator.ValidateAsync(request);
        if (!validationResults.IsValid)
        {
            return ValidationProblem(validationResults.ToString());
        }
        var response = await _handler.HandleAsync(request);

        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Strict : SameSiteMode.Lax
           //TODO put domain next time
        };
        HttpContext.Response.Cookies.Append("AccessToken", response.AccessToken!, cookieOptions);
        return response;
    }
    
    
}