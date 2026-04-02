
using System.ComponentModel.DataAnnotations;

namespace GigPH.Features.Auth.Register;

public class RegisterRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}