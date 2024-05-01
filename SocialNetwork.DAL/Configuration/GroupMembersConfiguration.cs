using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration
{
    public class GroupMembersConfiguration : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.User)
                .WithMany(i => i.GroupMembers);

            builder.HasMany(i => i.RoleGroup)
                .WithMany(r => r.GroupMembers);

        }
    }
}
