using PrjA.Domain;
using PrjA.Features.Genres;

namespace PrjA.Features.User.GetPublicProfile;

public class GetPublicProfileResponse
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }

    public ICollection<GenreDto> Genres { get; set; } = [];
    public ICollection<PublicProfileInstrumentDto> Instruments { get; set; } = [];
    public ICollection<PublicProfileSocialLinkDto> SocialLinks { get; set; } = [];

    public required PublicProfileLocationDto Location { get; set; }
}

public class PublicProfileInstrumentDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsMain { get; set; }
}

public class PublicProfileSocialLinkDto
{
    public required Platform Platform { get; set; }
    public required string Url { get; set; }
}

public class PublicProfileLocationDto
{
    public required string Country { get; set; }
    public required string ProvinceState { get; set; }
    public required string City { get; set; }
}
