namespace GigPH.Domain;

public class Media
{
    public OwnerType OwnerType { get; set; } 
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Key { get; set; }
    public long Size { get; set; } 
}