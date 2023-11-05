using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository.Interfaces;

public interface IBlackListRepository : IBasicRepository<BlackList>
{
    Task AddUserAsync(User wantToBanUser, BlackList blackList, CancellationToken cancellationToken = default);
    Task RemoveUserAsync(BlackList blacklist, CancellationToken cancellationToken = default);
    IQueryable<BlackList> GetAllBannedUserByUserId(int id);
}