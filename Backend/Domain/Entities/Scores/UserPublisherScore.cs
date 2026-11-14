namespace PrjA.Domain.Entities;

public class UserScorePublisher 
{
    public required string UserId { get; set; }
    public required string PublisherId { get; set; }
    public required int Score { get; set; }
    
}