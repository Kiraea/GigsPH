using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasOne<AppUser>().WithMany().HasForeignKey(c => c.InitiatedByUserId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(c => c.OwnerUserId);
    }
}