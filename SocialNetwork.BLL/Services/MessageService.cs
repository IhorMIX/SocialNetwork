using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.BLL.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<MessageService> _logger;
    private readonly IMapper _mapper;
    private readonly IChatRepository _chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IMessageReadStatusRepository _messageReadStatusRepository;
    private readonly INotificationRepository _notificationRepository;

    public MessageService(ILogger<MessageService> logger, IMapper mapper, IMessageRepository messageRepository, IChatRepository chatRepository, IChatMemberRepository chatMemberRepository, IMessageReadStatusRepository messageReadStatusRepository, INotificationRepository notificationRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _chatMemberRepository = chatMemberRepository;
        _messageReadStatusRepository = messageReadStatusRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<MessageModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _mapper.Map<MessageModel>(await _messageRepository.GetByIdAsync(id, cancellationToken));
    }

    public async Task<MessageModel> CreateMessage(int userId, int chatId, MessageModel messageModel, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var access = new List<ChatAccess>
        {
            ChatAccess.SendMessages
        };
        if (messageModel.FileModels?.Any() == true)
        {
            access.Add(ChatAccess.SendFiles);
        }

        var hasUserAccess = chatMemberDb!.Role.HasAccess(access);

        if (!hasUserAccess)
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageReadStatuses = new List<MessageReadStatus>();
        var chatMembers = chatDb!.ChatMembers.Where(i => i.User.Id != chatMemberDb.User.Id).ToList();
        foreach (var chatMember in chatMembers)
        {
            messageReadStatuses.Add(new()
            {
                ChatMemberId = chatMember.Id,
                ReadAt = DateTime.Now,
            });
        }
        
        var messageDb = await _messageRepository.CreateMessageAsync(new Message
        {
            Text = messageModel.Text,
            Files = _mapper.Map<List<FileEntity>>(messageModel.FileModels),
            CreatedAt = DateTime.Now,
            AuthorId = chatMemberDb!.Id,
            ChatId = chatId,
            ToReplyMessageId = null,
            Author = chatMemberDb,
            Chat = chatDb!,
            ToReplyMessage = null,
            Reactions = new List<Reaction>(),
            MessageReadStatuses = messageReadStatuses
        }, cancellationToken);
        
        return _mapper.Map<MessageModel>(messageDb);
    }
    
    public async Task DeleteMessageAsync(int userId, int chatId, int messageId, bool isForAuthor, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var hasAccess = chatMemberDb!.Role.HasAccess(new List<ChatAccess>{ ChatAccess.DelMessages });
        if (!hasAccess)
            throw new NoRightException($"You have no rights for it");
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        if (isForAuthor && messageDb!.AuthorId == chatMemberDb.Id)
        {
            messageDb!.IsDeleted = true;
            await _messageRepository.EditMessageAsync(messageDb, cancellationToken);
        }
        else
        {
            await _messageRepository.DeleteAsync(messageDb!, cancellationToken);
        }
    }

    public async Task<MessageModel> EditMessageAsync(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var access = new List<ChatAccess>
        {
            ChatAccess.SendMessages
        };
        if (messageModel.FileModels?.Any() == true)
        {
            access.Add(ChatAccess.SendFiles);
        }
        var hasUserAccess = chatMemberDb!.Role.HasAccess(access);
        if (!hasUserAccess)
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(messageDb, new UserNotFoundException($"Message with id-{messageId} not found"));

        
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<MessageModel, Message>.PropertyMap)
        {
            var messageProperty = propertyMap.Item1;
            var messageDbProperty = propertyMap.Item2;

            var messageSourceValue = messageProperty.GetValue(messageModel);
            var messageTargetValue = messageDbProperty.GetValue(messageDb);

            if (messageSourceValue != null && !messageSourceValue.Equals(0) && !ReferenceEquals(messageSourceValue, "") && !messageSourceValue.Equals(messageTargetValue))
            {
                messageDbProperty.SetValue(messageDb, messageSourceValue);
            }
        }
        
        messageDb = await _messageRepository.EditMessageAsync(messageDb!, cancellationToken);
        return _mapper.Map<MessageModel>(messageDb);
    }
    
    public async Task<MessageModel> ReplyMessageAsync(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var access = new List<ChatAccess>
        {
            ChatAccess.SendMessages
        };
        if (messageModel.FileModels?.Any() == true)
        {
            access.Add(ChatAccess.SendFiles);
        }
        var hasUserAccess = chatMemberDb!.Role.HasAccess(access);
        if (!hasUserAccess)
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messageToReply = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        var messageReadStatuses = new List<MessageReadStatus>();
        var chatMembers = chatDb!.ChatMembers.Where(i => i.User.Id != chatMemberDb.User.Id).ToList();
        foreach (var chatMember in chatMembers)
        {
            messageReadStatuses.Add(new()
            {
                ChatMemberId = chatMember.Id,
                ReadAt = DateTime.Now,
            });
        }
        
        var messageDb = await _messageRepository.CreateMessageAsync(new Message
        {
            Text = messageModel.Text,
            Files = _mapper.Map<List<FileEntity>>(messageModel.FileModels),
            CreatedAt = DateTime.Now,
            AuthorId = chatMemberDb!.Id,
            ChatId = chatId,
            ToReplyMessageId = messageToReply!.Id,
            Author = chatMemberDb,
            Chat = chatDb!,
            ToReplyMessage = messageToReply,
            Reactions = new List<Reaction>(),
            MessageReadStatuses = messageReadStatuses
        }, cancellationToken);
        
        var chatNotifications = chatDb.ChatMembers.Where(r => r.User.Id != chatMemberDb.User.Id).Select(chatMember => new MessageNotification
        {
            NotificationMessage = messageDb.Text,
            CreatedAt = DateTime.Now,
            ToUserId = chatMember.User.Id,
            InitiatorId = chatMemberDb.User.Id,
            ChatId = chatDb.Id,
            MessageId = messageDb.Id,
        }).ToList();
        
        await _notificationRepository.CreateNotifications(chatNotifications, cancellationToken);
        
        return _mapper.Map<MessageModel>(messageDb);
    }

    public async Task<List<MessageModel>> GetMessagesAsync(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        var messages = await _messageRepository.GetAll()
            .Where(m => m.ChatId == chatId && (!m.IsDeleted || m.AuthorId != chatMemberDb!.Id))
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<MessageModel>>(messages);
    }

    public async Task<MessageModel> GetLastMessageAsync(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        return _mapper.Map<MessageModel> (await _messageRepository.GetAll().Where(m => m.ChatId == chatId && m.IsDeleted == false).LastOrDefaultAsync(cancellationToken));
    }

    public async Task<MessageModel> GetByIdAsync(int userId, int chatId, int messageId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));

        return _mapper.Map<MessageModel> (await _messageRepository.GetAll()
            .Where(m => m.ChatId == chatId && m.Id == messageId && !m.IsDeleted)
            .SingleOrDefaultAsync(cancellationToken));
    }

    public async Task<List<MessageModel>> GetMessagesByTextAsync(int userId, int chatId, string text, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new UserNotFoundException($"Chat with id-{chatId} not found"));
        
        return _mapper.Map<List<MessageModel>> (await _messageRepository.GetAll()
            .Where(m => m.ChatId == chatId && m.Text.Contains(text) && !m.IsDeleted)
            .ToListAsync(cancellationToken));
    }

    public async Task ReadMessages(int userId, int chatId, IEnumerable<MessageModel> messageModels, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));

        List<MessageReadStatusModel> messageReadStatuses = messageModels.Where(i => i.Author!.Id != chatMemberDb!.Id && i.MessageReadStatuses != null)
            .Select(i => i.MessageReadStatuses!.SingleOrDefault(i => i.ChatMemberId == chatMemberDb!.Id))
            .ToList();
        messageReadStatuses = messageReadStatuses.Select(messageStatus =>
        {
            messageStatus.IsRead = true;
            messageStatus.ReadAt = DateTime.Now;
            return messageStatus;
        }).ToList();

        var messageIds = messageModels.Select(m => m.Id);
        var messNotification = await _notificationRepository.GetAll()
            .Where(r => messageIds.Contains((r as MessageNotification).Message.Id))
            .ToListAsync(cancellationToken);
        
        await _notificationRepository.RemoveNotification(messNotification, cancellationToken);
        
        await _messageReadStatusRepository.UpdateStatus(_mapper.Map<IEnumerable<MessageReadStatus>>(messageReadStatuses), cancellationToken);
    }

    public async Task<List<MessageNotificationModel>> CreateNotifications(MessageModel messageModel, IEnumerable<int> connectedUsersIds,
        CancellationToken cancellationToken = default)
    {
        var notifications = messageModel.Chat.ChatMembers!
            .Where(r => r.User.Id != messageModel.AuthorId && !connectedUsersIds.Contains(r.User.Id))
            .Select(chatMember =>  new MessageNotification
            {
                NotificationMessage = messageModel.Text,
                CreatedAt = DateTime.Now,
                ToUserId = chatMember.User.Id,
                InitiatorId = messageModel.Author!.User.Id,
                ChatId = messageModel.ChatId,
                MessageId = messageModel.Id,
            }).ToList();
        
        await _notificationRepository.CreateNotifications(notifications, cancellationToken);
        
        return _mapper.Map<List<MessageNotificationModel>>(notifications);
    }
}