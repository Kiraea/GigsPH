using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Auth.Login;



[ApiController]
[Route("/api/auth")]
public class LoginEndpoint : ControllerBase
{

    private readonly LoginHandler _handler;
    private readonly IValidator<LoginRequest> _validator;
    public LoginEndpoint(LoginHandler handler, IValidator<LoginRequest> validator)
    {
        _handler = handler;
        _validator = validator;
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
            Secure = false,
            SameSite = SameSiteMode.Strict
        };
        HttpContext.Response.Cookies.Append("AccessToken", response.AccessToken!, cookieOptions);
        return response;
    }
    
    
}