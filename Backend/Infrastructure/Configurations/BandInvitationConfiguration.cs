using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class BandInvitationConfiguration: IEntityTypeConfiguration<BandInvitation>
{
    public void Configure(EntityTypeBuilder<BandInvitation> builder)
    {
        builder.HasOne<Band>().WithMany().HasForeignKey(x => x.BandId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.InvitedByUserId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.InvitedToUserId);
        builder.HasIndex(x => new {x.BandId,  x.InvitedToUserId}).IsUnique()
            .HasFilter($"\"Status\" = {(int)InvitationStatus.Pending}");
        
    }
}