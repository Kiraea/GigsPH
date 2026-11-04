using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.Auth.Register;




[ApiController]
[Route("/api/auth")]
public class RegisterEndpoint  : ControllerBase
{
   private readonly IValidator<RegisterRequest> _validator;
   public RegisterEndpoint(IValidator<RegisterRequest> validator)
   {
      _validator = validator;
   }

   [HttpPost]
   public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
   {
      
      var result = await _validator.ValidateAsync(request);
      if (!result.IsValid)
      {
         string errors = string.Join(Environment.NewLine, result.Errors.Select(x => $"{x.ErrorMessage}"));
      }
      
      
      return Ok(result);

   }
}