using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        if (messageModel.Files?.Any() == true)
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
            Files = _mapper.Map<List<FileInMessage>>(messageModel.Files),
            CreatedAt = DateTime.Now,
            CreatorId = chatMemberDb.User.Id,
            SenderId = chatMemberDb.Id,
            ChatId = chatId,
            ToReplyMessageId = null,
            Creator = chatMemberDb.User,
            Sender = chatMemberDb,
            Chat = chatDb,
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
        _logger.LogAndThrowErrorIfNull(messageDb, new MessageNotFoundException($"Message with id-{messageId} not found"));
        
        if (isForAuthor && messageDb!.SenderId == chatMemberDb.Id)
        {
            messageDb!.IsDeleted = true;
            await _messageRepository.EditMessageAsync(messageDb, cancellationToken);
        }
        else
        {
            await _messageRepository.DeleteAsync(messageDb!, cancellationToken);
            
            var notifications = await _notificationRepository.GetAll()
                .Where(r => r is MessageNotification && ((MessageNotification)r).Message.Id == messageId)
                .ToListAsync(cancellationToken);

            await _notificationRepository.RemoveNotification(notifications, cancellationToken);
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
        if (messageModel.Files?.Any() == true)
        {
            access.Add(ChatAccess.SendFiles);
        }
        var hasUserAccess = chatMemberDb!.Role.HasAccess(access);
        
        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(messageDb, new MessageNotFoundException($"Message with id-{messageId} not found"));
        
        if (!hasUserAccess || messageDb!.CreatorId != userId)
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        foreach (var propertyMap in ReflectionHelper.WidgetUtil<MessageModel, Message>.PropertyMap)
        {
            var messageProperty = propertyMap.Item1;
            var messageDbProperty = propertyMap.Item2;

            var messageSourceValue = messageProperty.GetValue(messageModel);
            var messageTargetValue = messageDbProperty.GetValue(messageDb);


            if (messageSourceValue != null 
                && messageSourceValue!.GetType()!=typeof(DateTime) 
                && !messageSourceValue.Equals(0) 
                && !ReferenceEquals(messageSourceValue, "") 
                && !messageSourceValue.Equals(messageTargetValue))
            {
                if (messageSourceValue!.GetType() == typeof(List<FileInMessageModel>))
                {
                    messageDb!.Files = _mapper.Map<List<FileInMessage>>(messageSourceValue);
                }
                
                else
                {
                    messageDbProperty.SetValue(messageDb, messageSourceValue);
                }
            }
        }
        
        messageDb = await _messageRepository.EditMessageAsync(messageDb!, cancellationToken);
        return _mapper.Map<MessageModel>(messageDb);
    }
    
    public async Task<MessageModel> ReplyMessageAsync(int userId, int chatId, int messageId, MessageModel messageModel,
        CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with user id-{userId} in chat not found"));
        
        var access = new List<ChatAccess>
        {
            ChatAccess.SendMessages
        };
        if (messageModel.Files?.Any() == true)
        {
            access.Add(ChatAccess.SendFiles);
        }
        var hasUserAccess = chatMemberDb!.Role.HasAccess(access);
        if (!hasUserAccess)
        {
            throw new NoRightException($"You have no rights for it");
        }
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));

        var messageToReply = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(messageToReply, new MessageNotFoundException($"Message with id-{messageId} not found"));
        
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
            Files = _mapper.Map<List<FileInMessage>>(messageModel.Files),
            CreatedAt = DateTime.Now,
            CreatorId = chatMemberDb.User.Id,
            SenderId = chatMemberDb.Id,
            ChatId = chatId,
            ToReplyMessageId = messageToReply!.Id,
            Creator = chatMemberDb.User,
            Sender = chatMemberDb,
            Chat = chatDb,
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

    public async Task<IEnumerable<MessageModel>> GetMessagesAsync(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));

        var messages = await _messageRepository.GetAll()
            .Where(m => m.ChatId == chatId && (!m.IsDeleted || m.SenderId != chatMemberDb!.Id))
            .ToListAsync(cancellationToken);
        
        var messageModels = await ReadMessages(userId, chatId, _mapper.Map<List<MessageModel>>(messages), cancellationToken);
        
        return messageModels;
    }
    
    public async Task<PaginationResultModel<MessageModel>> GetMessagesAsync(int userId, int chatId, PaginationModel? pagination, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));

        List<Message> messagesDb;
        if (pagination == null)
        {
            messagesDb = await _messageRepository.GetAll()
                .Where(m => m.ChatId == chatId && (!m.IsDeleted || m.SenderId != chatMemberDb!.Id))
                .ToListAsync(cancellationToken);
        }
        else
        {
            messagesDb = await _messageRepository.GetAll()
                .Where(m => m.ChatId == chatId && (!m.IsDeleted || m.SenderId != chatMemberDb!.Id))
                .Pagination(pagination.CurrentPage, pagination.PageSize)
                .ToListAsync(cancellationToken);
        }
        var messageModels = await ReadMessages(userId, chatId, _mapper.Map<List<MessageModel>>(messagesDb), cancellationToken);
        
        var messages = new PaginationResultModel<MessageModel>
        {
            Data = messageModels,
            CurrentPage = pagination!.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = messagesDb.Count,
        };
        
        return messages;
    }

    public async Task<MessageModel> GetLastMessageAsync(int userId, int chatId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));
        
        return _mapper.Map<MessageModel>(await _messageRepository.GetAll().Where(m => m.ChatId == chatId && m.IsDeleted == false)
            .OrderBy(m=> m.CreatedAt)
            .LastOrDefaultAsync(cancellationToken));

    }

    public async Task<MessageModel> GetByIdAsync(int userId, int chatId, int messageId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));

        return _mapper.Map<MessageModel> (await _messageRepository.GetAll()
            .Where(m => m.ChatId == chatId && m.Id == messageId && !m.IsDeleted)
            .SingleOrDefaultAsync(cancellationToken));
    }

    public async Task<IEnumerable<MessageModel>> GetMessagesByTextAsync(int userId, int chatId, string text, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));
        
        return _mapper.Map<List<MessageModel>> (await _messageRepository.GetAll()
            .Where(m => m.ChatId == chatId && m.Text.Contains(text) && !m.IsDeleted)
            .ToListAsync(cancellationToken));
    }
    
    public async Task<PaginationResultModel<MessageModel>> GetMessagesByTextAsync(int userId, int chatId, string text, PaginationModel? pagination, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var chatDb = await _chatRepository.GetByIdAsync(chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatDb, new ChatNotFoundException($"Chat with id-{chatId} not found"));
        
        List<Message> messages;
        if (pagination == null)
        {
            messages = await _messageRepository.GetAll()
                .Where(m => m.ChatId == chatId && m.Text.Contains(text) && !m.IsDeleted)
                .ToListAsync(cancellationToken);

        }
        else
        {
            messages = await _messageRepository.GetAll()
                .Where(m => m.ChatId == chatId && m.Text.Contains(text) && !m.IsDeleted)
                .Pagination(pagination.CurrentPage, pagination.PageSize)
                .ToListAsync(cancellationToken);
        }
        
        return new PaginationResultModel<MessageModel>
        {
            Data = _mapper.Map<List<MessageModel>>(messages),
            CurrentPage = pagination!.CurrentPage,
            PageSize = pagination.PageSize,
            TotalItems = messages.Count,
        };
    }

    public async Task<IEnumerable<MessageModel>> ReadMessages(int userId, int chatId, List<MessageModel> messageModels, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var messageReadStatuses = messageModels
            .Where(i => i.Sender.Id != chatMemberDb!.Id && 
                        i.MessageReadStatuses!.Where(r => r.ChatMemberId == chatMemberDb.Id).Select(r => r.IsRead).Contains(false))
            .Select(i => i.MessageReadStatuses!.SingleOrDefault(i => i.ChatMemberId == chatMemberDb!.Id))
            .ToList();

        var messageTrueReadStatuses = messageReadStatuses.Select(messageStatus =>
        {
            messageStatus!.IsRead = true;
            messageStatus.ReadAt = DateTime.Now;
            return messageStatus;
        }).ToList();


        var messageIds = messageModels!.Select(m => m.Id);
        
        var messNotifications = await _notificationRepository.GetAll()
            .Where(r => r is MessageNotification && messageIds.Contains((r as MessageNotification)!.Message.Id))
            .Where(r=> r.ToUserId == userId)
            .ToListAsync(cancellationToken);
        
        await _notificationRepository.RemoveNotification(messNotifications, cancellationToken);
        await _messageReadStatusRepository.UpdateStatus(_mapper.Map<IEnumerable<MessageReadStatus>>(messageTrueReadStatuses), cancellationToken);
       
        var updatedMessages = await _messageRepository.GetAll().Where(r => messageIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<MessageModel>>(updatedMessages);
    }

    public async Task<MessageModel> ShareWithMessage(int userId, int messageId, int chatId, bool showCreator,
        CancellationToken cancellationToken = default)
    {
        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(messageDb, new MessageNotFoundException($"Message with id-{messageId} not found"));
        
        var chatMessageFrom = await _chatMemberRepository.GetByUserIdAndChatId(userId, messageDb!.ChatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMessageFrom, new UserNotFoundException($"Chat member with user id-{userId} in chat not found"));
        
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with user id-{userId} in chat not found"));
        
        
        var messageModel = _mapper.Map<MessageModel>(messageDb);
        var access = new List<ChatAccess>
        {
            ChatAccess.SendMessages
        };
        if (messageModel.Files?.Any() == true)
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
        
        List<string> filePaths = messageModel.Files!.Select(fe => fe.FilePath).ToList();

        List<FileInMessage> sharedFiles = new ();
        foreach (string filePath in filePaths)
        {
            sharedFiles.Add(new FileInMessage
            {
                FilePath = filePath
            });
        }
        
        var sharedMessageDb = await _messageRepository.CreateMessageAsync(new Message
        {
            Text = messageModel.Text,
            Files = sharedFiles,
            CreatedAt = DateTime.Now,
            CreatorId = messageModel.CreatorId,
            SenderId = chatMemberDb.Id,
            ChatId = chatId,
            ToReplyMessageId = null,
            Creator = messageDb.Creator,
            Sender = chatMemberDb,
            Chat = chatDb,
            ToReplyMessage = null,
            Reactions = new List<Reaction>(),
            MessageReadStatuses = messageReadStatuses
        }, cancellationToken);
        
        if (!showCreator)
        {
            sharedMessageDb.CreatorId = chatMemberDb.User.Id;
        }
        
        return _mapper.Map<MessageModel>(sharedMessageDb);
    }

    public async Task<List<MessageNotificationModel>> CreateNotification(MessageModel model, IEnumerable<int> connectedUsersIds,
        CancellationToken cancellationToken = default)
    {
        var notifications = model.Chat.ChatMembers!
            .Where(r => r.User.Id != model.SenderId && !connectedUsersIds.Contains(r.User.Id))
            .Select(chatMember =>  new MessageNotification
            {
                NotificationMessage = model.Text,
                CreatedAt = DateTime.Now,
                ToUserId = chatMember.User.Id,
                InitiatorId = model.Sender.User.Id,
                ChatId = model.ChatId,
                MessageId = model.Id,
            }).ToList();
        
        await _notificationRepository.CreateNotifications(notifications, cancellationToken);
        
        return _mapper.Map<List<MessageNotificationModel>>(notifications);
    }
}