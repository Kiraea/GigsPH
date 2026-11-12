using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace PrjA.Infrastructure.Email;

public class Email
{
    public required string ToEmail { get; init; }
    public required string ToDisplayName{ get; init; }
    public required string InputBody{ get; init; }
    public required string InputSubject { get; init; }

}


public class EmailSender
{
    private readonly SmtpOptions _options;

    public EmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public BuiltEmail BuildMessage(Email email)
    { 
        return BuiltEmail.Create(email, _options);
    }
}

public class BuiltEmail
{
    private readonly MimeMessage _message;
    private readonly SmtpOptions _options;
        
        // the reason why private so they cant do new BuiltEmail
        // only methods in the class can call builteMAIL
    private BuiltEmail(MimeMessage message, SmtpOptions options)
    {
        _message = message;
        _options = options;
    }

    
    // internal is basically so if u have a backendporject in this case /backend/csproj/ every class in that proj can call this one
    
    internal static BuiltEmail Create(Email email, SmtpOptions options)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(options);

        ArgumentException.ThrowIfNullOrWhiteSpace(email.ToEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(email.InputSubject);
        ArgumentException.ThrowIfNullOrWhiteSpace(email.InputBody);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.AppEmail);
        
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("PrjA", options.AppEmail));
        message.To.Add(new MailboxAddress(email.ToDisplayName, email.ToEmail));
        message.Subject = email.InputSubject;
        message.Body = new TextPart("plain")
        {
            Text = email.InputBody
        };
        return new BuiltEmail(message, options);
    }

    public async Task SendEmailAsync()
    {
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_options.SmtpHost, _options.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_options.AppEmail,  _options.AppPassword);
            
            var result = await client.SendAsync(_message);
            Console.WriteLine("Server Sent A Message" );
            await client.DisconnectAsync(true);
        }
    }

}