using SocialNetwork.BL.Models;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IMessageService : IBaseService<MessageModel>
{
    public Task<MessageModel> CreateMessage(int userId, int chatId, MessageModel messageModel,
        CancellationToken cancellationToken = default);

    public Task DeleteMessageAsync(int userId, int chatId, int messageId, bool isForAuthor, CancellationToken cancellationToken = default);

    public Task<MessageModel> EditMessageAsync(int userId, int chatId, int messageId, MessageModel messageModel, 
        CancellationToken cancellationToken = default);
    
    public Task<MessageModel> ReplyMessageAsync(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default);

    public Task<List<MessageModel>> GetMessagesAsync(int userId, int chatId, CancellationToken cancellationToken = default);
    public Task<MessageModel> GetLastMessageAsync(int userId, int chatId, CancellationToken cancellationToken = default);

    public Task<MessageModel> GetByIdAsync(int userId, int chatId, int messageId, CancellationToken cancellationToken = default);

}