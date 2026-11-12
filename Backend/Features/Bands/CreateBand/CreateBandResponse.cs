using PrjA.Domain.Entities;

namespace PrjA.Features.Bands.CreateBand;

public class CreateBandResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? description { get; set; }
    public BandRole Role { get; set; }
    
}