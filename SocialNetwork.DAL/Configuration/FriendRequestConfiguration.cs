using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.HasKey(f => new { f.SenderId, f.ReceiverId });
        
        builder.HasOne(i => i.Sender)
            .WithMany(i => i.Requests)
            .HasForeignKey(i => i.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(i => i.Receiver)
            .WithMany()
            .HasForeignKey(i => i.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}