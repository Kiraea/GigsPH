namespace PrjA.Features.Auth.CheckToken;

public class CheckTokenResponse
{
    public string Id { get; set; }
    public string? DisplayName { get; set; }
    public bool IsOnboarded { get; set; } 
}