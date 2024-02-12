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

public class NotificationService : INotificationService
{
    private readonly IUserService _userService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger, IMapper mapper, IUserService userService)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<BaseNotificationModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var friendRequestNotification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(friendRequestNotification,
            new NotificationNotFoundException($"Notification with this Id {id} not found"));
        return _mapper.Map<BaseNotificationModel>(friendRequestNotification);
    }
    
   public async Task<BaseNotificationModel> CreateNotification(BaseNotificationModel baseNotificationModel,
        CancellationToken cancellationToken = default)
    {
        var notificationId = await _notificationRepository.CreateNotification(
            _mapper.Map<BaseNotificationEntity>(baseNotificationModel), cancellationToken);
        return _mapper.Map<BaseNotificationModel>(await _notificationRepository.GetByIdAsync(notificationId, cancellationToken));
    }
   
    public async Task CreateNotifications(IEnumerable<BaseNotificationModel> baseNotificationModel,
        CancellationToken cancellationToken = default)
    {
        var notificationEntities = _mapper.Map<IEnumerable<BaseNotificationEntity>>(baseNotificationModel);
        await _notificationRepository.CreateNotifications(notificationEntities, cancellationToken);
    }
   
   public async Task RemoveNotification(int userId, int notificationId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notification with this Id {notificationId} not found"));
        await _notificationRepository.RemoveNotification(notification!, cancellationToken);
    }

    public async Task ReadNotification(int userId, int notificationId,
        CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetAll().FirstOrDefaultAsync(i => i.Id == notificationId && i.ToUserId == userDb!.Id, cancellationToken);
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notification with this Id {notificationId} and user {userDb!.Id} not found"));
        notification!.IsRead = true;
        await _notificationRepository.UpdateNotification(notification, cancellationToken);
    }

    public async Task<IEnumerable<BaseNotificationModel>> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetAll().Where(r => r.ToUserId == userId).ToListAsync(cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notifications with this user id {userId} not found"));
        return _mapper.Map<List<BaseNotificationModel>>(notification);
    }
    
    public async Task<IEnumerable<BaseNotificationModel>> GetBoxNotificationsByUserId(int userId, CancellationToken cancellationToken = default)
    {
        var userDb = await _userService.GetByIdAsync(userId, cancellationToken);
        _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
        var notification = await _notificationRepository.GetAll().Where(r => r.ToUserId == userId && !(r is MessageNotification)).ToListAsync(cancellationToken);
        
        _logger.LogAndThrowErrorIfNull(notification,
            new NotificationNotFoundException($"Notifications with this user id {userId} not found"));
        return _mapper.Map<List<BaseNotificationModel>>(notification);
    }
}