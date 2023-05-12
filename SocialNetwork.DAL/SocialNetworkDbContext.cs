using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
#pragma warning disable CS8618

namespace SocialNetwork.DAL;

public class SocialNetworkDbContext : DbContext
{
    public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }

    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialNetworkDbContext).Assembly);
    }
    
}