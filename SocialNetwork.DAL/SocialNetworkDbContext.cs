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
    
    public DbSet<Friendship> Friends { get; set; }
    
    public DbSet<FriendRequest> FriendRequests { get; set; }
    
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    
    public DbSet<ReadMessage> ReadMessages { get; set; }
    public DbSet<Reaction> Reactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialNetworkDbContext).Assembly);
    }
    
}