using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{

    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.HasMany(u => u.ChatMembers)
            .WithOne(u => u.Chat);

        builder.HasMany(c => c.Roles)
            .WithOne(r => r.Chat);
    }
}