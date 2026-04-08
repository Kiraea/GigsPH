namespace GigPH.Features.User.GetMyProfile;

public record GetMyProfileRequest
{
    public Guid UserId { get; set; }

}