using Microsoft.AspNetCore.Identity;

namespace PrjA.Features.Auth.Register;

public sealed class RegistrationRejectedException : Exception
{
    public IReadOnlyList<IdentityError> Errors { get; }

    public RegistrationRejectedException(IEnumerable<IdentityError> errors)
        : base("Account registration was rejected.")
    {
        Errors = errors.ToArray();
    }
}
