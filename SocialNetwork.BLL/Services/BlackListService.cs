using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.BLL.Helpers;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Repository;
using SocialNetwork.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Services.Interfaces;

namespace SocialNetwork.BLL.Services
{
    public class BlackListService : IBlackListService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IBlackListRepository _blackrepository;
        private readonly ILogger<BlackListService> _logger;
        private readonly IMapper _mapper;
        private readonly IFriendshipService _friendshipService;
        private readonly IFriendRequestRepository _friendRequestRepository;

        public BlackListService(IUserRepository userRepository, IUserService userService, 
            IBlackListRepository blackListRepository, ILogger<BlackListService> logger, IMapper mapper, IFriendshipService friendshipService,
            IFriendRequestRepository friendRequestRepository)
        {
            _userRepository = userRepository;
            _userService = userService;
            _blackrepository = blackListRepository;
            _logger = logger;
            _mapper = mapper;
            _friendshipService = friendshipService;
            _friendRequestRepository = friendRequestRepository;
        }

        public async Task<BlackListModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var banneduserdb = await _blackrepository.GetByIdAsync(id, cancellationToken);
            _logger.LogAndThrowErrorIfNull(banneduserdb, new BannedUserNotFoundException("BannedUser not found"));
            var banneduserModel = _mapper.Map<BlackListModel>(banneduserdb);
            return banneduserModel;
        }

        public async Task AddUserToBlackListAsync(int userId, int wantToBanId, CancellationToken cancellationToken = default)
        {
            var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
            var user2Db = await _userRepository.GetByIdAsync(wantToBanId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException($"User with this Id {userId} not found"));
            _logger.LogAndThrowErrorIfNull(user2Db, new UserNotFoundException($"User with this Id {userId} not found"));

            if (userDb!.Id != user2Db!.Id)
            {
                var blacklist = new BlackList()
                {
                    UserId = userDb!.Id,
                    BannedUserId = user2Db!.Id,
                };
                await _friendRequestRepository.DeleteFriendRequestAsync(new FriendRequest()
                {
                    SenderId = userDb!.Id,
                    ReceiverId = user2Db!.Id
                }, cancellationToken);

                await _friendshipService.DeleteFriendshipAsync(userDb.Id, user2Db.Id, cancellationToken);
                await _blackrepository.AddUserAsync(user2Db, blacklist, cancellationToken);
                
               
            }
            else
            {
                _logger.LogError("You cant add to blacklist yourself");
                throw new BanYourselfException("You cant ban yourself");
            }
        }

        public async Task DeleteUserFromBlackListAsync(int userId, int bannedUserId, CancellationToken cancellationToken = default)
        {
            var userModel = await _userService.GetByIdAsync(userId, cancellationToken);
            var user2Model = await _userService.GetByIdAsync(bannedUserId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(userModel, new UserNotFoundException("User not found"));
            _logger.LogAndThrowErrorIfNull(user2Model, new UserNotFoundException("User not found"));
            var blacklist = new BlackList()
            {
                UserId = userModel!.Id,
                BannedUserId = user2Model!.Id,
            };
            await _blackrepository.RemoveUserAsync(blacklist, cancellationToken);

        }

        public async Task<IEnumerable<UserModel>> FindBannedUserByNameSurname(int searchingUserId, string nameSurname, CancellationToken cancellationToken = default)
        {
            var searchingUser = await _userRepository.GetByIdAsync(searchingUserId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(searchingUser, new UserNotFoundException("User not found"));

            string[] parts = nameSurname.Split();

            IEnumerable<User>? matchingUsers = null;
            if (parts.Length == 1)
            {
                string name = parts[0].ToLower();
                matchingUsers = await _blackrepository.GetAllBannedUserByUserId(searchingUser.Id)
                    .Where(f => f.BannedUser.Profile.Name.ToLower().StartsWith(name)
                              || f.BannedUser.Profile.Surname.ToLower().StartsWith(name))
                    .Select(f => f.BannedUser)
                    .ToListAsync(cancellationToken);
            }
            else if (parts.Length == 2)
            {
                string firstName = parts[0].ToLower();
                string lastName = parts[1].ToLower();

                matchingUsers = await _blackrepository.GetAllBannedUserByUserId(searchingUser.Id)
                    .Where(f => (f.BannedUser.Profile.Name.ToLower().StartsWith(firstName)
                                  && f.BannedUser.Profile.Surname.ToLower().StartsWith(lastName))
                               || (f.BannedUser.Profile.Name.ToLower().StartsWith(lastName)
                                  && f.BannedUser.Profile.Surname.ToLower().StartsWith(firstName)))
                    .Select(f => f.BannedUser)
                    .ToListAsync(cancellationToken);
            }

            var blacklist = _mapper.Map<IEnumerable<UserModel>>(matchingUsers);
            return blacklist;
        }


        public async Task<IEnumerable<UserModel>> GetAllBannedUser(int userId, CancellationToken cancellationToken = default)
        {
            var bannedUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
            _logger.LogAndThrowErrorIfNull(bannedUser, new BannedUserNotFoundException("Banned user not found"));

            var blackLists = await _blackrepository.GetAllBannedUserByUserId(bannedUser.Id).Select(f => f.BannedUser).ToListAsync(cancellationToken);
            var userModels = _mapper.Map<IEnumerable<UserModel>>(blackLists);
            return userModels;
        }



        public async Task<bool> IsBannedUser(int userId, int bannedUserId, CancellationToken cancellationToken = default)
        {
            var userDb = await _userRepository.GetByIdAsync(userId, cancellationToken);
            var user2Db = await _userRepository.GetByIdAsync(bannedUserId, cancellationToken);

            _logger.LogAndThrowErrorIfNull(userDb, new UserNotFoundException("User not found"));
            _logger.LogAndThrowErrorIfNull(user2Db, new UserNotFoundException("User not found"));

            var isBanned = await _blackrepository
                .GetAllBannedUserByUserId(userId)
                .AnyAsync(i => i.UserId == userId && i.BannedUserId == bannedUserId, cancellationToken);

            return isBanned;
        }

    }
}