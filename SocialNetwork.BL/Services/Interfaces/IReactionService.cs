using SocialNetwork.BL.Models;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IReactionService : IBaseService<ReactionModel>
{
    public Task AddReaction(int userId, int messageId, ReactionModel reactionModel, CancellationToken cancellationToken = default);
    public Task RemoveReaction(int userId, int chatId, int readcionId, CancellationToken cancellationToken = default);
}