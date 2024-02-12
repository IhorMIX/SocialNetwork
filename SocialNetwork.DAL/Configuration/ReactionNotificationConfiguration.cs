using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class ReactionNotificationConfiguration : IEntityTypeConfiguration<ReactionNotification>
{
    public void Configure(EntityTypeBuilder<ReactionNotification> builder)
    {
        builder.HasOne(m => m.Reaction)
            .WithOne(c => c.Notification)
            .OnDelete(DeleteBehavior.NoAction);
    }
}