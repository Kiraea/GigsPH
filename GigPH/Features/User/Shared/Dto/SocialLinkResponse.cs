namespace GigPH.Features.User.Shared.Dto;

public record SocialLinkResponse()
{
    public Guid SocialLinkId { get; init; }
    public string Url { get; init; }
};
