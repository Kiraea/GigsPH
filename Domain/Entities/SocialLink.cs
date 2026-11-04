namespace PrjA.Domain.Entities;

public class SocialLink
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Platform Platform { get; set; }
    public string Url { get; set; }

    
    // the reason why no required  tldr
    // if u set both to required now u have to type both and chance of mismatch (low tho)
    // u can only set 1, 
    // answer
    // 
    //You have only the user ID: set AppUserId.
    //•
    //You have the user object: set AppUser.
    
    /*
     *
     * but making the FK the value you always set when creating a social link:
public required string AppUserId { get; set; }
public AppUser? AppUser { get; set; }
When creating a link, set AppUserId:
var link = new SocialLink
{
    AppUserId = userId,
    Url = url
};
If you have the AppUser object instead of a separate ID, it already has an ID, so use AppUserId = user.Id.
Here, required on AppUserId makes the compiler require an owner ID when creating the link. AppUser? means the user object might not be loaded into this particular C# object. It does not make the database relationship optional: the non-nullable AppUserId is what makes the relationship required. EF Core allows a required relationship to have a nullable navigation when the related object may not be loaded. Microsoft’s EF Core guidance also explains this distinction.
So my corrected recommendation is: required ID, nullable navigation, set the ID when creating; load the navigation only when you need the user object. null! is another EF Core pattern, but it hides the possibility that the navigation hasn’t been loaded. For where you are now, AppUser? makes that easier to see and check.
     */
    public required string AppUserId { get; set; } 
    public AppUser AppUser { get; set; } = null!;
}
