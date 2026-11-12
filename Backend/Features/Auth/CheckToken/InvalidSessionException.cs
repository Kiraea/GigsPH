namespace PrjA.Features.Auth.CheckToken;

public sealed class InvalidSessionException : Exception
{
    public InvalidSessionException() : base("Your session is no longer valid.")
    {
    }
}
