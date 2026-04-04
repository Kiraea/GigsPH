namespace GigPH.Features.User.Onboard;

public record OnboardRequest
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; init; }
    public string? FirstName{ get; init; }
    public string? LastName{ get; init; }
    public string? Description { get; init; }
    
    // lets just make everything required so fix in validators
}