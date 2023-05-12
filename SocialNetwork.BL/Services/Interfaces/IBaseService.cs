namespace SocialNetwork.BL.Services.Interfaces;

public interface IBaseService<TModel> where TModel : class
{
    Task<TModel?> GetById(int id, CancellationToken cancellationToken = default);
}