using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class UserPublisherScoreConfiguration : IEntityTypeConfiguration<UserPublisherScore>
{
    public void Configure(EntityTypeBuilder<UserPublisherScore> builder)
    {
        builder.HasKey(x => new { x.UserId, x.PublisherId });
    }
}