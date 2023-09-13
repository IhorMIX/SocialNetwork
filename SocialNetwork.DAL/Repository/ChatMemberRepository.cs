using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Services;

namespace SocialNetwork.DAL.Repository;

public class ChatMemberRepository : IChatMemberRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly CacheService<ChatMember?> _cacheService;
    public ChatMemberRepository(SocialNetworkDbContext socialNetworkDbContext, CacheService<ChatMember?> cacheService)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _cacheService = cacheService;
    }
    public IQueryable<ChatMember> GetAll()
    {
        return _socialNetworkDbContext.ChatMembers
            .Include(c => c.Chat)
            .Include(c => c.Role.OrderBy(r => r.Rank))
            .Include(c => c.User)
            .ThenInclude(u=>u.Profile)
            .AsQueryable();
    }

    public async Task<ChatMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrSetAsync($"Chat member - {id}", async (token) =>
        {
            return await _socialNetworkDbContext.ChatMembers
                .Include(c => c.Chat)
                .Include(c => c.Role)
                .Include(c => c.User)
                .FirstOrDefaultAsync( i => i.Id == id, token);
        }, cancellationToken);
    }

    public async Task SetRole(List<ChatMember> chatMembers, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.ChatMembers.UpdateRange(chatMembers);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}