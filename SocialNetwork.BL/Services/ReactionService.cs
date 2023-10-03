using AutoMapper;
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
    private readonly IUserRepository _userRepository;
    private readonly ReactionRepository _reactionRepository;
    private readonly MessageRepository _messageRepository;
    private readonly ILogger<FriendshipService> _logger;
    private readonly IMapper _mapper;
    private readonly IChatRepository _chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository;


    public ReactionService(IUserRepository userRepository, ReactionRepository reactionRepository, ILogger<FriendshipService> logger, IMapper mapper, IChatRepository chatRepository, IChatMemberRepository chatMemberRepository, MessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _reactionRepository = reactionRepository;
        _logger = logger;
        _mapper = mapper;
        _chatRepository = chatRepository;
        _chatMemberRepository = chatMemberRepository;
        _messageRepository = messageRepository;
    }

    public async Task<ReactionModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _mapper.Map<ReactionModel>(await _reactionRepository.GetByIdAsync(id, cancellationToken));
    }

    public async Task AddReaction(int userId, int messageId, ReactionModel reactionModel, CancellationToken cancellationToken = default)
    {
        var chatMemberDb = await _chatMemberRepository.GetByUserIdAndChatId(userId, messageId, cancellationToken);
        _logger.IsExists(chatMemberDb, new UserNotFoundException($"Chat member with id-{userId} not found"));
        
        var messageDb = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        _logger.IsExists(messageDb, new UserNotFoundException($"Chat with id-{messageId} not found"));

        await _reactionRepository.CreateReactionAsync(new Reaction
        {
            Type = reactionModel.Type,
            MessageId = messageDb!.Id,
            AuthorId = chatMemberDb!.Id,
            Author = chatMemberDb!,
            Message = messageDb
        }, cancellationToken);

    }

    public Task RemoveReaction(int userId, int chatId, int reactionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}