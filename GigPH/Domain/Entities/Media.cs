namespace GigPH.Domain;

public class Media
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public OwnerType OwnerType { get; set; } 
    
    public string Key { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public long FileSize { get; set; }
}