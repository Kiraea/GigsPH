namespace GigPH.Features.User.Shared.Dto;

public record LocationResponse
{
    
    public Guid Id { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string ProvinceState { get; set; }

}