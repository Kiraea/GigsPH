using GigPH.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GigPH.Infrastructure.Config;

public class BandUserConfig : IEntityTypeConfiguration<BandUser>
{
    public void Configure(EntityTypeBuilder<BandUser> builder)
    {
        
        builder.HasKey(bu=> new {bu.AppUserId, bu.BandId });
        builder.HasOne(bu => bu.Band)
            .WithMany(b => b.BandUsers)
            .HasForeignKey(bu => bu.BandId);
        builder.HasOne(bu => bu.AppUser)
            .WithMany(au => au.BandUsers)
            .HasForeignKey(au=> au.AppUserId);

        
    }
}