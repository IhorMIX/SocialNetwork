using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Configuration
{
    public class RoleGroupConfiguration : IEntityTypeConfiguration<RoleGroup>
    {
        public void Configure(EntityTypeBuilder<RoleGroup> builder)
        {
            builder.HasMany(i => i.RoleAccesses)
                .WithOne(i => i.RoleGroup)
                .HasForeignKey(i => i.RoleId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasMany(i => i.GroupMembers)
                .WithMany(i => i.RoleGroup);
            builder.HasOne(i => i.Group)
                .WithMany(i => i.RoleGroups)
                .HasForeignKey(i => i.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
