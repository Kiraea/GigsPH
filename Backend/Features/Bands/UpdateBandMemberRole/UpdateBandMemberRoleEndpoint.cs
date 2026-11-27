using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

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

    [HttpPatch("{bandId:guid}/members/{targetUserId}/role")]
    public async Task<ActionResult<UpdateBandMemberRoleResponse>> UpdateBandMemberRole(
        [FromRoute] Guid bandId,
        [FromRoute] string targetUserId,
        [FromBody] UpdateBandMemberRoleRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (bandId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(bandId), "Band ID must not be empty.");
            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(targetUserId))
        {
            ModelState.AddModelError(nameof(targetUserId), "Target user ID must not be empty.");
            return ValidationProblem(ModelState);
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

        var response = await _handler.HandleAsync(bandId, targetUserId, request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
