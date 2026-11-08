using PrjA.Domain;

namespace PrjA.Features.User.UpdateProfile;

public class ProfileInstrumentRequestDto
{
    public Guid Id { get; set; }
    public bool IsMain { get; set; }
}
public class ProfileLocationRequestDto 
{
    public required string Country { get; set; }
    public required string ProvinceState { get; set; }
    public required string City { get; set; }
}

public class ProfileSocialLinkRequestDto 
{
    public required Platform Platform { get; set; }
    public required string Url { get; set; }
}




public class ProfileInstrumentResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsMain { get; set; }
}
public class ProfileLocationResponseDto 
{
    public required string Country { get; set; }
    public required string ProvinceState { get; set; }
    public required string City { get; set; }
}

public class ProfileSocialLinkResponseDto 
{
    public required Platform Platform { get; set; }
    public required string Url { get; set; }
}



public class ProfileGenreResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
