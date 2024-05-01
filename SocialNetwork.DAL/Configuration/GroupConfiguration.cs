using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
namespace SocialNetwork.DAL.Configuration
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {

        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasMany(u => u.GroupMembers)
                .WithOne(u => u.Group);

            builder.HasMany(c => c.RoleGroups)
                .WithOne(r => r.Group);

        }
    }
}