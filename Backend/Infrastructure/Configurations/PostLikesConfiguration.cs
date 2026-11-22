using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class PostLikesConfiguration : IEntityTypeConfiguration<PostLikes>
{
    public void Configure(EntityTypeBuilder<PostLikes> builder)
    {
        builder.HasKey(x => new { x.PostId, x.UserId });
        
    }
}