namespace GigPH.Features.Auth.Login;

public record LoginRequest
{
    public string? UsernameOrEmail{ get; init; }
    public string? Password { get; init; }
    
}