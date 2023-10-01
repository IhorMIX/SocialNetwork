using Microsoft.EntityFrameworkCore;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.DAL.Repository;

public class ReadMessageRepository : IReadMessageRepository
{
    private readonly SocialNetworkDbContext _socialNetworkDbContext;

    public ReadMessageRepository(SocialNetworkDbContext socialNetworkDbContext)
    {
        _socialNetworkDbContext = socialNetworkDbContext;
    }
    
    public IQueryable<ReadMessage> GetAll()
    {
        return _socialNetworkDbContext.ReadMessages.Include(i=>i.ChatMember)
            .Include(i => i.Message)
            .AsQueryable();
    }

    public async Task<ReadMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _socialNetworkDbContext.ReadMessages.Include(i=>i.ChatMember)
            .Include(i => i.Message)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateReactionAsync(ReadMessage readMessage, CancellationToken cancellationToken = default)
    {
        await _socialNetworkDbContext.ReadMessages.AddAsync(readMessage, cancellationToken);
        await _socialNetworkDbContext.SaveChangesAsync(cancellationToken);
    }
}