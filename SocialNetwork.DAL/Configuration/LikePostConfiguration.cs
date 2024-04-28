using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class LikePostConfiguration : IEntityTypeConfiguration<LikePost>
{
    public void Configure(EntityTypeBuilder<LikePost> builder)
    {
        builder.HasOne(i => i.User)
            .WithMany(i => i.LikedPosts)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
        
        builder.HasOne(i => i.Post)
            .WithMany(i => i.Likes)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.ClientCascade);
        

    }
}