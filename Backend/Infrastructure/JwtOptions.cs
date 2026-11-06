namespace PrjA.Infrastructure;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience{ get; set; }
    public string ExpirationInMinutes { get; set; }
    public string SecretKey { get; set; }
}