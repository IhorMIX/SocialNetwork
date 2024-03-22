using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class ChatRepository : IChatRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public ChatRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<Chat> GetAll()
    {
        return _socialNetworkDbContext.Chats
            .Include(i => i.ChatMembers)
            .Include(i => i.Roles)
            .AsQueryable();
    }

    public async Task<Chat?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Chats
            .Include(i => i.ChatMembers)
            .Include(c => c.Roles)
            .Include(r => r.Messages)
            .Include(c => c.ChatMembers).ThenInclude(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<int> CreateChat(Chat chat, CancellationToken cancellationToken = default)
    {
        var chatEntity = await _socialNetworkDbContext.Chats.AddAsync(chat, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return chatEntity.Entity.Id;
    }

    public async Task DeleteChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Chats.Remove(chat);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddChatMemberAsync(ChatMember сhatMember, Chat chat,
        CancellationToken cancellationToken = default)
    {
        chat.ChatMembers.Add(сhatMember);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddChatMemberAsync(List<ChatMember> сhatMembers, Chat chat,
        CancellationToken cancellationToken = default)
    {
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

    public async Task DelChatMemberAsync(List<ChatMember> chatMembers, Chat chat,
        CancellationToken cancellationToken = default)
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
    }
}