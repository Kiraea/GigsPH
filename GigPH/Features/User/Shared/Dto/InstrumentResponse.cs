namespace GigPH.Features.User.Shared.Dto;

public record InstrumentResponse
{
    
    public Guid Id { get; init; } 
    public string Name { get; init; }
}