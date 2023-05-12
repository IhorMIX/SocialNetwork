namespace SocialNetwork.DAL.Repository.Interfaces;

public interface IBasicRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll();

    Task<TEntity?> GetById(int id, CancellationToken cancellationToken = default);
}