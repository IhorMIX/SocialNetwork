    using SocialNetwork.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BL.Services.Interfaces
{
    public interface IBlackListService : IBaseService<BlackListModel>
    {
        Task AddUserToBlackListAsync(int userId, int bannedUserId, CancellationToken cancellationToken = default);
        Task DeleteUserFromBlackListAsync(int userId, int bannedUserId, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserModel>> GetAllBannedUser(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserModel>> FindBannedUserByNameSurname(int userId, string nameSurname, CancellationToken cancellationToken = default);
        Task<bool> IsBannedUser(int userId, int bannedUserId, CancellationToken cancellationToken = default);
    }
}