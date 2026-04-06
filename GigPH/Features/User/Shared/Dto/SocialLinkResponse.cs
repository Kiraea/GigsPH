namespace GigPH.Features.User.Shared.Dto;

public record SocialLinkResponse()
{
    public Guid Id { get; init; }
    public string Url { get; init; }
};
