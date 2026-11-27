using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PrjA.Infrastructure;

namespace PrjA.Features.Uploads;



[ApiController]
[Route("/api")]
public class UploadEndpoint:ControllerBase
{
    private readonly UploadHandler _handler;
    private readonly IValidator<UploadRequest> _validator;
    public UploadEndpoint(UploadHandler handler, IValidator<UploadRequest> validator)
    {
        _handler = handler;
        _validator = validator;

    }


    [HttpPost("upload")]
    public async Task<ActionResult<UploadResponse>> Upload([FromBody] UploadRequest request)
    {

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var response = await _validator.ValidateAsync(request);
        if (!response.IsValid)
        {
            var modelStateDictionary = new ModelStateDictionary();

            foreach (var error in response.Errors)
            {
                modelStateDictionary.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(modelStateDictionary);
        }

        var result = await _handler.HandleAsync(request, userId);

        return result.isSuccess ? Ok(result.Value) : result.ToProblemDetails(this);


    }
}