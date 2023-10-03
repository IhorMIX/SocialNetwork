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

    public async Task<Message> CreateMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        var messageDb = (await _socialNetworkDbContext.Messages.AddAsync(message, cancellationToken)).Entity;
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return messageDb;
    }

    public async Task<Message> EditMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        var messageDb = _socialNetworkDbContext.Messages.Update(message).Entity;
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
        return messageDb;
    }

    public async Task DeleteAsync(Message message, CancellationToken cancellationToken = default)
    {
        _socialNetworkDbContext.Messages.Remove(message);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}