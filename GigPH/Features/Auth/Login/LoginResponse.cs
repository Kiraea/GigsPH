namespace GigPH.Features.Auth.Login;

public record LoginResponse
{
    public Guid UserId { get; init; }
    public string? Email { get; init ; }
    public string? AccessToken { get; init; }
    
}