namespace GigPH.Features.User.Shared.Dto;

public record LocationRequest
{
    public string Country { get; init; }
    public string City { get; init; }
    public string ProvinceState { get; init; }

}
