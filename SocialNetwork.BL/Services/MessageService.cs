using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.BL.Helpers;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BL.Services;

public class MessageService : IMessageService
{
    private readonly IUserRepository _userRepository;
    private readonly MessageRepository _messageRepository;
    private readonly ILogger<FriendshipService> _logger;
    private readonly IMapper _mapper;
    private readonly IChatRepository _chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IReadMessageRepository _readMessageRepository;

    public MessageService(IUserRepository userRepository, ILogger<FriendshipService> logger, IMapper mapper, MessageRepository messageRepository, IChatRepository chatRepository, IChatMemberRepository chatMemberRepository, IReadMessageRepository readMessageRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _chatMemberRepository = chatMemberRepository;
        _readMessageRepository = readMessageRepository;
    }

    public async Task<MessageModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _mapper.Map<MessageModel>(await _messageRepository.GetByIdAsync(id, cancellationToken));
    }

    public async Task<MessageModel> CreateMessage(int userId, int chatId, MessageModel messageModel, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var userInChat = chatMemberDb!.Role.Where(c => 
            c is { SendMessages: true, SendFiles: true});
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));
        
        var messageDb = await _messageRepository.CreateMessageAsync(new Message
        {
            Text = messageModel.Text,
            Files = messageModel.Files,
            CreatedAt = DateTime.Now,
            IsRead = false,
            IsEdited = false,
            AuthorId = chatMemberDb!.Id,
            ChatId = chatId,
            ToReplyMessageId = null,
            Author = chatMemberDb,
            Chat = chatDb!,
            ToReplyMessage = null,
            ReadMessages = new List<ReadMessage>(),
            Reactions = new List<Reaction>()
        }, cancellationToken);
        
        return _mapper.Map<MessageModel>(messageDb);
    }

    public async Task DeleteMessage(int userId, int chatId, int messageId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var userInChat = chatMemberDb!.Role.Where(c => 
            c is { DelMessages: true});
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        await _messageRepository.DeleteAsync(messageDb!, cancellationToken);
    }

    public async Task<MessageModel> EditModel(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var userInChat = chatMemberDb!.Role.Where(c => 
            c is { SendMessages: true, SendFiles: true});
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.IsExists(messageDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        messageDb = await _messageRepository.EditMessageAsync(new Message
        {
            Text = messageModel.Text,
            Files = messageModel.Files,
            IsEdited = true,
        }, cancellationToken);
        
        return _mapper.Map<MessageModel>(messageDb);
    }

    public async Task<MessageModel> ReplyMessage(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var userInChat = chatMemberDb!.Role.Where(c => 
            c is { SendMessages: true, SendFiles: true});
        _logger.IsExists(userInChat, new NoRightException($"You have no rights for it"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageToReply = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        var messageDb = await _messageRepository.CreateMessageAsync(new Message
        {
            Text = messageModel.Text,
            Files = messageModel.Files,
            CreatedAt = DateTime.Now,
            IsRead = false,
            IsEdited = false,
            AuthorId = chatMemberDb!.Id,
            ChatId = chatId,
            ToReplyMessageId = messageToReply!.Id,
            Author = chatMemberDb,
            Chat = chatDb!,
            ToReplyMessage = messageToReply,
            ReadMessages = new List<ReadMessage>(),
            Reactions = new List<Reaction>()
        }, cancellationToken);
        
        return _mapper.Map<MessageModel>(messageDb);
    }

    public async Task<List<MessageModel>> GetMessages(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        // await _readMessageRepository.AddReadAsync(new ReadMessage
        // {
        //     Id = 0,
        //     ReadAt = DateTime.Now,
        //     ChatMemberId = chatMemberDb!.Id,
        //     MessageId = 0,
        //     Message = null,
        //     ChatMember = chatMemberDb
        // }, cancellationToken);
        
        
        return _mapper.Map<List<MessageModel>>(await _messageRepository.GetAll().Where(m => m.ChatId == chatId).ToListAsync(cancellationToken));
    }

    public async Task<MessageModel> GetLastMessage(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.IsExists(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        return _mapper.Map<MessageModel> (await _messageRepository.GetAll().Where(m => m.ChatId == chatId).LastOrDefaultAsync(cancellationToken));
    }
}