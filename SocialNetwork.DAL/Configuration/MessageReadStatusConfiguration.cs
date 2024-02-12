using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class MessageReadStatusConfiguration : IEntityTypeConfiguration<MessageReadStatus>
{
    public void Configure(EntityTypeBuilder<MessageReadStatus> builder)
    {
        builder.HasKey(f => new { f.ChatMemberId, f.MessageId });
    }
}