namespace GigPH.Features.Auth.Login;

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    
}