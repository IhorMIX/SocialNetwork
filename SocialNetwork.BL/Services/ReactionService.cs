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

public class ReactionService : IReactionService
{
    private readonly IReactionRepository _reactionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<FriendshipService> _logger;
    private readonly IMapper _mapper;
    private readonly IChatMemberRepository _chatMemberRepository;


    public ReactionService(IReactionRepository reactionRepository, ILogger<FriendshipService> logger, IMapper mapper, IChatMemberRepository chatMemberRepository, IMessageRepository messageRepository)
    {
        _reactionRepository = reactionRepository;
        _logger = logger;
        _mapper = mapper;
        _chatMemberRepository = chatMemberRepository;
        _messageRepository = messageRepository;
    }

    public async Task<ReactionModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _mapper.Map<ReactionModel>(await _reactionRepository.GetByIdAsync(id, cancellationToken));
    }

    public async Task<ReactionModel> AddReaction(int userId, int messageId, ReactionModel reactionModel, CancellationToken cancellationToken = default)
    {
        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(messageDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, messageDb!.ChatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));

        var reactionDb = await _reactionRepository.GetAll()
            .FirstOrDefaultAsync(r => r.MessageId == messageDb.Id && r.AuthorId == chatMemberDb!.Id, cancellationToken: cancellationToken);
        if (reactionDb is not null)
        {
            reactionDb.Type = reactionModel.Type;
            await _reactionRepository.EditReactionAsync(reactionDb, cancellationToken);
        }
        else
        {
            reactionDb = await _reactionRepository.CreateReactionAsync(new Reaction
            {
                Type = reactionModel.Type,
                MessageId = messageDb!.Id,
                AuthorId = chatMemberDb!.Id,
                Author = chatMemberDb!,
                Message = messageDb
            }, cancellationToken);
        }
        return _mapper.Map<ReactionModel>(reactionDb);
    }

    public async Task RemoveReaction(int userId, int chatId, int reactionId, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, chatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var reactionDb = await _reactionRepository.GetByIdAsync(reactionId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(reactionDb, new UserNotFoundException($"Reaction with id-{reactionId} not found"));

        await _reactionRepository.DeleteReactionAsync(reactionDb!, cancellationToken);
    }

    public async Task<List<ReactionModel>> GetReactionByMessage(int userId, int messageId, ReactionModel reactionModel,
        CancellationToken cancellationToken = default)
    {
        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(messageDb, new UserNotFoundException($"Message with id-{messageId} not found"));
        
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, messageDb!.ChatId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));

        var reactionsDb = await _reactionRepository.GetAll().Where(r => r.MessageId == messageDb.Id && r.AuthorId == chatMemberDb.Id)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ReactionModel>>(reactionsDb);
    }
}