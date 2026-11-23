namespace PrjA.Infrastructure.Email;

public class SmtpOptions
{
    public string AppEmail { get; set; }
    public string AppPassword { get; set; }
    public int Port { get; set; }
    public string SmtpHost { get; set; }
}