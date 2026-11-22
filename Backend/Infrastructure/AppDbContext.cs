using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }
    
    //onModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
        

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entries = ChangeTracker.Entries<ITimestamp>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).ToList();

        var now = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = now;
                entry.Property("UpdatedAt").CurrentValue = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<BandInvitation> BandInvitations { get; set; }
    public DbSet<MediaUpload> MediaUploads { get; set; }
    
    
    
    public DbSet<UserBlock> UserBlocks { get; set; }
    public DbSet<UserBand> UserBands { get; set; }
    
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationMember> ConversationMembers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<GroupChatInvitation> GroupChatInvitations{ get; set; }

    
    
    public DbSet<MusicListing> MusicListings { get; set; }
    public DbSet<MusicListingSlot > MusicListingSlots { get; set; }
    public DbSet<MusicListingApplication> MusicListingApplications { get; set; }
    
    

    public DbSet<UserInstrument> UserInstruments { get; set; }
    public DbSet<UserGenreScore> UserGenreScores { get; set; }
    public DbSet<UserInstrumentScore> UserInstrumentScores { get; set; }
    public DbSet<UserPublisherScore> UserPublisherScores { get; set; }
    public DbSet<Band> Bands { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
}
