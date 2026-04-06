namespace GigPH.Features.User.GetProfileById;

public record GetMyProfileRequest
{
    public Guid UserId { get; init; }
    public bool IncludeSocialLinks { get; init; } = false;
    public bool IncludeBands { get; init; } = false;
    public bool IncludeGenres { get; init; } = false;
    public bool IncludeInstruments { get; init; } = false;
}