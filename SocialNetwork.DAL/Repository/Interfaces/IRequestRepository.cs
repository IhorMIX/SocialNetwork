using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repository.Interfaces
{
    public interface IRequestRepository : IBasicRepository<BaseRequestEntity>
    {
        public Task<int> CreateRequestAsync(BaseRequestEntity baseRequestEntity, CancellationToken cancellationToken = default);
        public Task<bool> DeleteRequestAsync(BaseRequestEntity requestEntity, CancellationToken cancellationToken = default);
        public Task<bool> DeleteAllRequestAsync(int userId, CancellationToken cancellationToken = default);
        public IQueryable<BaseRequestEntity> GetAllRequest(int id);
        public Task<bool> RequestExists(BaseRequestEntity requestEntity, CancellationToken cancellationToken = default);
    }
}
