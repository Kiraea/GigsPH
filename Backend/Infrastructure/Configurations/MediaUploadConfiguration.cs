using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class MediaUploadConfiguration : IEntityTypeConfiguration<MediaUpload>
{
    public void Configure(EntityTypeBuilder<MediaUpload> builder)
    {
        builder.HasOne<AppUser>().WithMany().HasForeignKey(m => m.RequestedByUserId);
        

    }
}