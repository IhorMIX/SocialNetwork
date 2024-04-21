using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class UserPostConfiguration : IEntityTypeConfiguration<UserPost>
{
    public void Configure(EntityTypeBuilder<UserPost> builder)
    {
        builder.HasOne(i => i.User)
            .WithMany(i => i.Posts)
            .HasForeignKey(i => i.UserId);
    }
}