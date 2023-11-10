using SocialNetwork.BLL.Models;

namespace SocialNetwork.BLL.Services.Interfaces;

public interface IReactionService : IBaseService<ReactionModel>
{
    public Task<ReactionModel> AddReaction(int userId, int messageId, ReactionModel reactionModel, CancellationToken cancellationToken = default);
    public Task RemoveReaction(int userId, int chatId, int reactionId, CancellationToken cancellationToken = default);

    public Task<List<ReactionModel>> GetReactionByMessage(int userId, int messageId, ReactionModel reactionModel,
        CancellationToken cancellationToken = default);
}