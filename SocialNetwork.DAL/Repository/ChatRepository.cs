using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Services;

namespace SocialNetwork.DAL.Repository;

public class ChatRepository : IChatRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;
    private readonly CacheService<Chat?> _cacheService;
    private readonly CacheService<ChatMember?> _cacheServiceChatMember;
    public ChatRepository(SocialNetworkDbContext socialNetworkDbContext, CacheService<Chat?> cacheService, CacheService<ChatMember?> cacheServiceChatMember)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
        _cacheService = cacheService;
        _cacheServiceChatMember = cacheServiceChatMember;
    }

    public IQueryable<Chat> GetAll()
    {
        return _socialNetworkDbContext.Chats
            .Include(i => i.ChatMembers)
            .Include(i =>i.Roles)
            .AsQueryable();
    }

    public async Task<Chat?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrSetAsync($"Chat - {id}", async (token) =>
        {
            return await _socialNetworkDbContext.Chats
                .Include(i => i.ChatMembers)
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(i => i.Id == id, token);
        }, cancellationToken);
    }

    public async Task<int> CreateChat(Chat chat, CancellationToken cancellationToken = default)
    {
        var chatEntity = await _socialNetworkDbContext.Chats.AddAsync(chat, cancellationToken); 
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.GetOrSetAsync($"Chat-{chatEntity.Entity.Id}", (_) => Task.FromResult(chat)!, cancellationToken);
        return chatEntity.Entity.Id;
        
    }
    
    public async Task DeleteChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Chats.Remove(chat);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveFromCacheAsync($"Chat-{chat.Id}", cancellationToken);
    }
    
    public async Task AddChatMemberAsync(ChatMember сhatMember, Chat chat, CancellationToken cancellationToken = default)
    {
        if (chat.ChatMembers == null)
            chat.ChatMembers = new List<ChatMember>();
        chat.ChatMembers.Add(сhatMember);
        _socialNetworkDbContext.ChatMembers.AddRange(сhatMember);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddChatMemberAsync(List<ChatMember> сhatMembers, Chat chat, CancellationToken cancellationToken = default)
    {
        if (chat.ChatMembers == null)
            chat.ChatMembers = new List<ChatMember>();
        _socialNetworkDbContext.ChatMembers.AddRange(сhatMembers);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
    

    public async Task DelChatMemberAsync(int userId, Chat chat, CancellationToken cancellationToken = default)
    {
        var сhatMember = await _socialNetworkDbContext.ChatMembers
            .Include(c => c.Chat)
            .Include(c => c.Role)
            .FirstOrDefaultAsync(i => i.User.Id == userId && i.Chat.Id == chat.Id, cancellationToken);
        
        chat.ChatMembers?.Remove(сhatMember!);
        _socialNetworkDbContext.Chats.Update(chat);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DelChatMemberAsync(List<ChatMember> chatMembers, Chat chat, CancellationToken cancellationToken = default)
    {
        foreach (var c in chatMembers)
        {
            chat.ChatMembers?.Remove(c);
        }
        _socialNetworkDbContext.Chats.Update(chat);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task EditChat(Chat chat, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Chats.Update(chat);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        await _cacheService.UpdateAsync($"Chat-{chat.Id}", (_) => Task.FromResult(chat)!, cancellationToken);
    }
}