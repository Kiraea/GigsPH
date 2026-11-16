namespace PrjA.Domain.Entities;


public interface ITimestamp {
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
}