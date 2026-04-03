
using System.ComponentModel.DataAnnotations;

namespace GigPH.Features.Auth.Register;

public record RegisterRequest
{
    public string? Email { get; init ; }
    public string? Password { get; init; }
}