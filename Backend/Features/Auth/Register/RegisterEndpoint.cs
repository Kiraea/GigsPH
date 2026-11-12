using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.Auth.Register;




[ApiController]
[Route("/api/auth")]
public class RegisterEndpoint  : ControllerBase
{
   private readonly IValidator<RegisterRequest> _validator;
   private readonly RegisterHandler _handler;
   public RegisterEndpoint(IValidator<RegisterRequest> validator, RegisterHandler handler)
   {
      _validator = validator;
      _handler = handler;
   }

   [HttpPost("register")]
   public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
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
      
      try
      {
         var response = await _handler.HandleAsync(request);
         return Ok(response);
      }
      catch (RegistrationRejectedException exception)
      {
         foreach (var error in exception.Errors)
         {
            ModelState.AddModelError(error.Code, error.Description);
         }

         return ValidationProblem(ModelState);
      }

   }
}
