using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;


namespace SocialNetwork.DAL.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.RoleAccesses)
            .HasConversion(
                v => string.Join(',', v),
                v => !string.IsNullOrEmpty(v)
                    ? v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<ChatAccess>)
                        .ToList()
                    : new List<ChatAccess>())
            .Metadata.SetValueComparer(new CollectionValueComparer<ChatAccess>());
    }
}

