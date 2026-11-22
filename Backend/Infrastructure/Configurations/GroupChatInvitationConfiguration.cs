using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class GroupChatInvitationConfiguration : IEntityTypeConfiguration<GroupChatInvitation>
{
    public void Configure(EntityTypeBuilder<GroupChatInvitation> builder)
    {
        builder.HasOne(gci => gci.Conversation).WithMany().HasForeignKey(gci => gci.ConversationId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(gci => gci.InvitedByUserId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(gci => gci.InvitedToUserId);

        builder.HasIndex(gci => new { gci.ConversationId, gci.InvitedToUserId }).IsUnique()
            .HasFilter($"\"Status\" = {(int)GroupChatInvitationStatus.Pending}");
        


    }
}