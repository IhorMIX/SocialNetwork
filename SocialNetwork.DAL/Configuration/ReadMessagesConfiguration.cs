using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class ReadMessagesConfiguration : IEntityTypeConfiguration<ReadMessage>
{
    public void Configure(EntityTypeBuilder<ReadMessage> builder)
    {
        builder.HasKey(mr => mr.Id);
        
        builder.HasOne(mr => mr.ChatMember)
            .WithMany(u => u.MessagesRead)
            .HasForeignKey(mr => mr.ChatMemberId)
            .OnDelete(DeleteBehavior.Restrict);

       
        builder.HasOne(mr => mr.Message)
            .WithMany(m => m.ReadMessages)
            .HasForeignKey(mr => mr.MessageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}