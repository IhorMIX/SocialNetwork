using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class BasePostConfiguration : IEntityTypeConfiguration<BasePostEntity>
{
    public void Configure(EntityTypeBuilder<BasePostEntity> builder)
    {
        builder.HasMany(r => r.Likes)
            .WithOne(r => r.Post)
            .HasForeignKey(r => r.PostId);
        
        builder.HasMany(r => r.Comments)
            .WithOne(r => r.Post)
            .HasForeignKey(r => r.PostId);
        
        builder.HasMany(r => r.Files)
            .WithOne(r => r.Post)
            .HasForeignKey(r => r.PostId);
        
    }
}