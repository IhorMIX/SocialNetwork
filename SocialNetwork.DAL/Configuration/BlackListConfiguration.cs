using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;

public class BlackListConfiguration : IEntityTypeConfiguration<BlackList>
{
    public void Configure(EntityTypeBuilder<BlackList> builder)
    {
        builder.HasKey(bl => new { bl.UserId, bl.BannedUserId });

        builder.HasOne(bl => bl.User)
            .WithMany(u => u.BlackLists)
            .HasForeignKey(bl => bl.UserId)
          .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(bl => bl.BannedUser)
            .WithMany()
            .HasForeignKey(bl => bl.BannedUserId)
             .OnDelete(DeleteBehavior.NoAction);

    }
}