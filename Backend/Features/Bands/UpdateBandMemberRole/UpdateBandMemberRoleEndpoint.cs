using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.Bands.UpdateBandMemberRole;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class UpdateBandMemberRoleEndpoint : ControllerBase
{
    private readonly UpdateBandMemberRoleHandler _handler;
    private readonly IValidator<UpdateBandMemberRoleRequest> _validator;

    public UpdateBandMemberRoleEndpoint(
        UpdateBandMemberRoleHandler handler,
        IValidator<UpdateBandMemberRoleRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPatch("members/role")]
    public async Task<ActionResult<UpdateBandMemberRoleResponse>> UpdateBandMemberRole(
        [FromBody] UpdateBandMemberRoleRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(request, userId);
        return Ok(response);
    }
}