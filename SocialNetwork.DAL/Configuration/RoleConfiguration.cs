using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.DAL.Configuration;

public class RoleConfiguration :  IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany(i => i.RoleAccesses)
            .WithOne(i => i.Role)
            .HasForeignKey(i => i.RoleId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}