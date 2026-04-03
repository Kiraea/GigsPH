namespace GigPH.Features.User.Shared.Dto;

public record BandResponse()
{
    public Guid BandId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
};