using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class MusicListingConfiguration : IEntityTypeConfiguration<MusicListing>
{
    public void Configure(EntityTypeBuilder<MusicListing> builder)
    {
        builder.HasOne(ml => ml.Genre).WithMany().HasForeignKey(ml => ml.GenreId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(ml => ml.UserId);
        
    }
}