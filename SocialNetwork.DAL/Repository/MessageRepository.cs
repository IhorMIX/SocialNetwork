using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public MessageRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }

    public IQueryable<Message> GetAll()
    {
        return _socialNetworkDbContext.Messages.Include(m => m.Chat)
            .Include(m => m.Author)
            .Include(m => m.Reactions)
            .AsQueryable();
    }

    public async Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.Messages.Include(m => m.Chat)
            .Include(m => m.Author)
            .Include(m => m.Reactions)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateMessage(Message message, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.Messages.AddAsync(message, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EditMessage(Message message, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Messages.Update(message);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}