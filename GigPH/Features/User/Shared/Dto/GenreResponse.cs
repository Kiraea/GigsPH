namespace GigPH.Features.User.Shared.Dto;

public record GenreResponse
{
    
    public Guid Id { get; init; } 
    public string Name { get; init; }
}