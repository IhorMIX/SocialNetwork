using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class FriendsConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => new { f.UserId, f.FriendId });
        
        builder.HasOne(i => i.User)
            .WithMany(i => i.Friends)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(i => i.FriendUser)
            .WithMany()
            .HasForeignKey(i => i.FriendId)
            .OnDelete(DeleteBehavior.NoAction);
       
    }
}