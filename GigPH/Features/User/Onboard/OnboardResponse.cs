namespace GigPH.Features.User.Onboard;

public record OnboardResponse
{
    public string? DisplayName { get; init; }
    public string? FirstName{ get; init; }
    public string? LastName{ get; init; }
    public string? Description { get; init; }
}