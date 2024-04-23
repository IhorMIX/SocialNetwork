using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        //builder.HasKey(f => new { f.Id, f.SenderId, f.ToUserId });

        //builder.Property(f => f.Id)
        //    .ValueGeneratedOnAdd();

        builder.HasOne(i => i.ToUser)
            .WithMany()
            .HasForeignKey(i => i.ToUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}