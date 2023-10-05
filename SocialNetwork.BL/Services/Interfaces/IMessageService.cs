using SocialNetwork.BL.Models;

namespace SocialNetwork.BL.Services.Interfaces;

public interface IMessageService : IBaseService<MessageModel>
{
    public Task<MessageModel> CreateMessage(int userId, int chatId, MessageModel messageModel,
        CancellationToken cancellationToken = default);

    public Task DeleteMessage(int userId, int chatId, int messageId, CancellationToken cancellationToken = default);

    public Task<MessageModel> EditMessage(int userId, int chatId, int messageId, MessageModel messageModel, 
        CancellationToken cancellationToken = default);
    
    public Task<MessageModel> ReplyMessage(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default);

    public Task<List<MessageModel>> GetMessages(int userId, int chatId, CancellationToken cancellationToken = default);
    public Task<MessageModel> GetLastMessage(int userId, int chatId, CancellationToken cancellationToken = default);
}