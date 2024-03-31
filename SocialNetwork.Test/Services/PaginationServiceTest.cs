using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Interfaces;
using SocialNetwork.DAL.Repository.Interfaces;
using SocialNetwork.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Test.Helpers;
using SocialNetwork.BLL.Models;
using SocialNetwork.BLL.Exceptions;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.Test.Services
{
    public class PaginationServiceTest : BaseMessageTestService<IBlackListService, BlackListService> 
    {
        
        protected override void SetUpAdditionalDependencies(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
        
            services.AddScoped<IFriendshipService, FriendshipService>();
            services.AddScoped<IFriendRequestService, FriendRequestService>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();

            services.AddScoped<IBlackListService, BlackListService>();
            services.AddScoped<IBlackListRepository, BlackListRepository>();
        
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            
            base.SetUpAdditionalDependencies(services);
        }
        
        [Test]
        public async Task AddUsersToBlackList_UsersAddAndMakePaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var justUser = await UserModelHelper.CreateTestDataAsync(userService);
            var wantToBanUser = await UserModelHelper.CreateTestDataAsync(userService);
            var wantToBanUser2 = await UserModelHelper.CreateTestDataAsync(userService);

            justUser = await userService.GetByIdAsync(justUser.Id);
            wantToBanUser = await userService.GetByIdAsync(wantToBanUser.Id);
            wantToBanUser2 = await userService.GetByIdAsync(wantToBanUser2.Id);

            Assert.That(justUser, Is.Not.EqualTo(null));
            Assert.That(wantToBanUser, Is.Not.EqualTo(null));
            Assert.That(wantToBanUser2, Is.Not.EqualTo(null));

            var paginationModel = new PaginationModel
            {
                CurrentPage = 2,
                PageSize = 1
            };

            await Service.AddUserToBlackListAsync(justUser!.Id, wantToBanUser!.Id);
            await Service.AddUserToBlackListAsync(justUser!.Id, wantToBanUser2!.Id);
            var userInBlackList = await Service.GetAllBannedUser(justUser.Id, paginationModel);

            Assert.That(userInBlackList.Data.FirstOrDefault()?.Id, Is.EqualTo(wantToBanUser2.Id));

        }


        [Test]
        public async Task CreateGroupChat_AddMemberAndMakePaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            var user2 = await UserModelHelper.CreateTestDataAsync(userService);
            var user3 = await UserModelHelper.CreateTestDataAsync(userService);
            var user4 = await UserModelHelper.CreateTestDataAsync(userService);
            user1 = await userService.GetUserByLogin(user1.Login);
            user2 = await userService.GetUserByLogin(user2.Login);
            user3 = await userService.GetUserByLogin(user3.Login);
            user4 = await userService.GetUserByLogin(user4.Login);
            Assert.That(user1, Is.Not.EqualTo(null));
            Assert.That(user2, Is.Not.EqualTo(null));
            Assert.That(user3, Is.Not.EqualTo(null));
            Assert.That(user4, Is.Not.EqualTo(null));

            var chatService = ServiceProvider.GetRequiredService<IChatService>();
            await chatService.CreateGroupChat(user1!.Id, new ChatModel
            {
                Name = "Chat2",
                Logo = "null",
                IsGroup = true,
            });

            var paginationModel = new PaginationModel
            {
                CurrentPage = 1,
                PageSize = 1
            };

            var chat = await chatService.FindChatByName(user1.Id, paginationModel, "Chat2");
            Assert.That(chat.Data.Count() == 1);

            await chatService.AddUsers(user1.Id, chat.Data.First().Id, new List<int> { user2!.Id, user3!.Id, user4!.Id });
            paginationModel.PageSize = 4;
            chat = await chatService.FindChatByName(user1.Id, paginationModel, "Chat2");
            Assert.That(chat.Data.First().ChatMembers!.Count == 4);
        }


        [Test]
        public async Task CreateGroupChats_GetChatsWithPaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var chatService = ServiceProvider.GetRequiredService<IChatService>();
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            user1 = await userService.GetUserByLogin(user1.Login);
            Assert.That(user1, Is.Not.EqualTo(null));

            var paginationModel = new PaginationModel
            {
                CurrentPage = 2,
                PageSize = 2
            };
            await chatService.CreateGroupChat(user1!.Id, new ChatModel
            {
                Name = "Chats1",
                Logo = "null",
                IsGroup = true,
            });
            await chatService.CreateGroupChat(user1.Id, new ChatModel
            {
                Name = "Chats2",
                Logo = "null",
                IsGroup = true,
            });
            await chatService.CreateGroupChat(user1.Id, new ChatModel
            {
                Name = "Chats3",
                Logo = "null",
                IsGroup = true,
            });

            var chat = await chatService.GetAllChats(user1.Id, paginationModel);
            Assert.That(chat.Data.Count() == 1);
        }


        [Test]
        public async Task CreateRole_MakePaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var chatService = ServiceProvider.GetRequiredService<IChatService>();
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            user1 = await userService.GetUserByLogin(user1.Login);
            Assert.That(user1, Is.Not.EqualTo(null));
 

            await chatService.CreateGroupChat(user1!.Id, new ChatModel
            {
                Name = "Chat4",
                Logo = "null",
                IsGroup = true,
            });

            var paginationModel = new PaginationModel
            {
                CurrentPage = 1,
                PageSize = 10
            };
            var chat = (await chatService.FindChatByName(user1.Id, paginationModel, "Chat4")).Data.First();

            Assert.That(chat is not null);

            await chatService.AddRole(user1.Id, chat!.Id, new RoleModel
            {
                RoleName = "Role2",
                RoleColor = "black",
            });

            await chatService.AddRole(user1.Id, chat.Id, new RoleModel
            {
                RoleName = "Role3",
                RoleColor = "blue",
            });

            var roles = await chatService.GetAllChatRoles(user1.Id, paginationModel, chat.Id);
            Assert.That(roles.Data.Count() == 3);
        }

        [Test]
        public async Task CreateGroupChat_GetChatMembersAndMakePaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var chatService = ServiceProvider.GetRequiredService<IChatService>();
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            var user2 = await UserModelHelper.CreateTestDataAsync(userService);
            user1 = await userService.GetUserByLogin(user1.Login);
            user2 = await userService.GetUserByLogin(user2.Login);
            Assert.That(user1, Is.Not.EqualTo(null));
            Assert.That(user2, Is.Not.EqualTo(null));

            await chatService.CreateGroupChat(user1!.Id, new ChatModel
            {
                Name = "Chat4",
                Logo = "null",
                IsGroup = true,
            });

            var paginationModel = new PaginationModel
            {
                CurrentPage = 1,
                PageSize = 2
            };
            var chat = (await chatService.FindChatByName(user1.Id, paginationModel, "Chat4")).Data.First();

            Assert.That(chat is not null);

            await chatService.AddUsers(user1.Id, chat!.Id, new List<int> { user2!.Id});

            var members = await chatService.GetChatMembers(user1.Id, paginationModel, chat.Id);

            Assert.That(members.Data.Count() == 2);
        }

        [Test]
        public async Task GetAllRequest_MakePaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var friendRequestService = ServiceProvider.GetRequiredService<IFriendRequestService>();
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            var user2 = await UserModelHelper.CreateTestDataAsync(userService);
            var user3 = await UserModelHelper.CreateTestDataAsync(userService);
            var createdUser1 = await userService.GetUserByLogin(user1.Login);
            var createdUser2 = await userService.GetUserByLogin(user2.Login);
            var createdUser3 = await userService.GetUserByLogin(user3.Login);
            Assert.That(user1, Is.Not.EqualTo(null));
            Assert.That(user2, Is.Not.EqualTo(null));
            Assert.That(user3, Is.Not.EqualTo(null));

            var paginationModel = new PaginationModel
            {
                CurrentPage = 1,
                PageSize = 10
            };

            await friendRequestService.SendRequest(createdUser2!.Id, createdUser1!.Id);
            await friendRequestService.SendRequest(createdUser3!.Id, createdUser1!.Id);
            Assert.That(friendRequestService.GetByUsersId(createdUser2.Id, createdUser1.Id), Is.Not.EqualTo(null));
            Assert.That(friendRequestService.GetByUsersId(createdUser3.Id, createdUser1.Id), Is.Not.EqualTo(null));

            var request = await friendRequestService.GetAllIncomeRequest(createdUser1.Id, paginationModel);
            Assert.That(request.Data.Count() == 2);
        }

        [Test]
        public async Task GetAllIncomes_MakePaginationModel_SuccessReturned()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var friendRequestService = ServiceProvider.GetRequiredService<IFriendRequestService>();
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            var user2 = await UserModelHelper.CreateTestDataAsync(userService);
            var user3 = await UserModelHelper.CreateTestDataAsync(userService);
            var createdUser1 = await userService.GetUserByLogin(user1.Login);
            var createdUser2 = await userService.GetUserByLogin(user2.Login);
            var createdUser3 = await userService.GetUserByLogin(user3.Login);
            Assert.That(user1, Is.Not.EqualTo(null));
            Assert.That(user2, Is.Not.EqualTo(null));
            Assert.That(user3, Is.Not.EqualTo(null));

            var paginationModel = new PaginationModel
            {
                CurrentPage = 1,
                PageSize = 10
            };

            await friendRequestService.SendRequest(createdUser1!.Id, createdUser2!.Id);
            await friendRequestService.SendRequest(createdUser1!.Id, createdUser3!.Id);
            Assert.That(friendRequestService.GetByUsersId(createdUser1.Id, createdUser2.Id), Is.Not.EqualTo(null));
            Assert.That(friendRequestService.GetByUsersId(createdUser1.Id, createdUser3.Id), Is.Not.EqualTo(null));

            var request = await friendRequestService.GetAllSentRequest(createdUser1.Id, paginationModel);
            Assert.That(request.Data.Count() == 2);
        }

        [Test]
        public async Task GetAllFriends_UserFoundAndMakePagination_ReturnFriends()
        {
            var userService = ServiceProvider.GetRequiredService<IUserService>();
            var friendService = ServiceProvider.GetRequiredService<IFriendshipService>();
           
            var user1 = await UserModelHelper.CreateTestDataAsync(userService);
            var createdUser1 = await userService.GetUserByLogin(user1.Login);
            Assert.That(user1, Is.Not.EqualTo(null));

            var user2 = await UserModelHelper.CreateTestDataAsync(userService);
            var createdUser2 = await userService.GetUserByLogin(user2.Login);
            Assert.That(user2, Is.Not.EqualTo(null));

            var user3 = await UserModelHelper.CreateTestDataAsync(userService);
            var createdUser3 = await userService.GetUserByLogin(user3.Login);
            Assert.That(user3, Is.Not.EqualTo(null));

            var paginationModel = new PaginationModel
            {
                CurrentPage = 1,
                PageSize = 1
            };

            await friendService.AddFriendshipAsync(createdUser1!.Id, createdUser2!.Id);

            await friendService.AddFriendshipAsync(createdUser1!.Id, createdUser3!.Id);

            Assert.That(
                friendService.GetAllFriends(user1.Id, paginationModel),
                Is.Not.EqualTo(null));

            var friendList = await friendService.GetAllFriends(user1.Id, paginationModel);
            Assert.That(friendList.Data.Count() == 1);
        }

    }
}
