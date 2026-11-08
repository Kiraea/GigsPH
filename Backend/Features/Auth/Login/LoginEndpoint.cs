using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
            string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage));
        }

        var response = await _handler.HandleAsync(request);
        Response.Cookies.Append("access_token", response.AccessToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
        });
        
       
        
        return Ok(response);

    }
    
    
}
