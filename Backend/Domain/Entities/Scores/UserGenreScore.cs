namespace PrjA.Domain.Entities;

public class UserScoreGenre 
{
    public required string UserId { get; set; }
    public required Guid GenreId { get; set; }
    public required int Score { get; set; }
}