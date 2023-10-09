using System.Text.Json;
using Microsoft.EntityFrameworkCore;
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
                d => !d.Any() ? "[]" : JsonSerializer.Serialize(d, JsonSerializerOptions.Default),
                s => string.IsNullOrEmpty(s) ? new List<ChatAccess>() : JsonSerializer.Deserialize<List<ChatAccess>>(s, JsonSerializerOptions.Default)!);
    }
}