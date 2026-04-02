namespace GigPH.Infrastructure;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationInMinutes { get; set; }
    public string SigningKey { get; set; }
}