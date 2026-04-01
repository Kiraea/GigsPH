using GigPH.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GigPH.Infrastructure.Config;

public class AppUserLinkConfig : IEntityTypeConfiguration<AppUserLink>
{
    public void Configure(EntityTypeBuilder<AppUserLink> builder)
    {
        builder.HasOne(aus => aus.AppUser)
            .WithMany(au => au.AppUserLinks)
            .HasForeignKey(aus => aus.AppUserId);
    }
}