using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne<Conversation>().WithMany(c => c.Messages).HasForeignKey(f => f.ConversationId);
        builder.HasOne(m => m.SenderUser).WithMany().HasForeignKey(f => f.SenderUserId);
        builder.HasIndex(m => m.ClientMessageId).IsUnique();
        
    }
}