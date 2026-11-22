namespace PrjA.Domain.Entities;

public class UserGenreScore
{
    public required string UserId { get; set; }
    public required Guid GenreId { get; set; }
    public required int Score { get; set; }
}
