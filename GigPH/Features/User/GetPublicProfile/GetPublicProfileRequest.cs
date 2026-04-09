namespace GigPH.Features.User.GetPublicProfile;

public record GetPublicProfileRequest
{
    public Guid UserId { get; set; }
};