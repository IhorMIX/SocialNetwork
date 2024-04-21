using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class CommentPostConfiguration : IEntityTypeConfiguration<CommentPost>
{
    public void Configure(EntityTypeBuilder<CommentPost> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasOne(i => i.User)
            .WithMany(i => i.CommentedPost)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
        
        builder.HasOne(i => i.Post)
            .WithMany(i => i.Comments)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}