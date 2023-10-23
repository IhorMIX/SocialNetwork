using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.Web.Models;
using Profile = AutoMapper.Profile;

namespace SocialNetwork.Web.MapperConfiguration
{
    public class MapperModelsConfig : Profile
    {
        public MapperModelsConfig()
        {
            CreateMap<UserCreateViewModel, UserModel>();

            CreateMap<ProfileCreateViewModel, ProfileModel>();
            CreateMap<FriendshipModel, Friendship>().ReverseMap();
            CreateMap<FriendRequestModel, FriendRequest>().ReverseMap();
            CreateMap<BlackListModel, BlackList>().ReverseMap();

            CreateMap<FriendRequestModel, FriendRequestViewModel>()
                .ReverseMap();
            
            CreateMap<ProfileFriendViewModel, ProfileModel>()
                .ForMember(dest =>dest.Birthday, opt=> opt.Ignore())
                .ForMember(dest =>dest.Description, opt=> opt.Ignore())
                .ReverseMap();
            CreateMap<FriendViewModel, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest =>dest.Login, opt=> opt.Ignore())
                .ForMember(dest =>dest.Password, opt=> opt.Ignore())
                .ForMember(dest =>dest.IsEnabled, opt=> opt.Ignore())
                .ForMember(dest =>dest.AuthorizationInfo, opt=> opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id));

            CreateMap<ProfileBannedUserModel, ProfileModel>()
                 .ForMember(dest => dest.Birthday, opt => opt.Ignore())
                 .ForMember(dest => dest.Description, opt => opt.Ignore())
                 .ReverseMap();
            CreateMap<BannedUserViewModel, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Login, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.IsEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfo, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id));

            CreateMap<ProfileCreateViewModel, ProfileModel>();  

            CreateMap<ProfileUpdateViewModel, ProfileModel>(); //created map model based on ProfileUpdateViewModel
            CreateMap<UserUpdateViewModel, UserModel>(); //created map model based on UserUpdateViewModel

            CreateMap<ProfileModel, ProfileViewModel>(); //created map model based on ProfileGetViewModel
            CreateMap<UserModel, UserViewModel>(); //created map model based on UserGetViewModel


            CreateMap<UserModel, User>()
                .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfoId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<AuthorizationInfoModel, AuthorizationInfo>().ReverseMap();
            CreateMap<ProfileModel, SocialNetwork.DAL.Entity.Profile>().ReverseMap();
            CreateMap<ChatModel, Chat>().ReverseMap();

            
            CreateMap<ChatMemberModel, ChatMember>().ReverseMap();
            
            CreateMap<Role, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.MapFrom(src => src.ChatMembers.Select(cm => cm.User.Id)))
                .ForMember(dest => dest.RoleAccesses, from => from.MapFrom(f => f.RoleAccesses.Select(i => i.ChatAccess)))
                .ReverseMap();

            CreateMap<ChatAccess, RoleChatAccess>()
                .ForMember(dest => dest.ChatAccess, from => from.MapFrom(f => f));
          
            
            CreateMap<ChatCreateViewModel, ChatModel>()
                .ForMember(dest => dest.ChatMembers, opt=>opt.Ignore())
                .ForMember(dest => dest.Roles, opt=>opt.Ignore())
                .ReverseMap();

            CreateMap<CreateRoleModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<RoleEditModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<RoleViewModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ChatMemberRoleViewModel, RoleModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.UsersIds, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<ChatMemberViewModel, ChatMemberModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<ChatViewModel, ChatModel>()
                .ForMember(dest => dest.ChatMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ChatEditModel, ChatModel>()
                .ForMember(dest => dest.ChatMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ChatModel, ChatEditModel>()
                .ForMember(dest => dest.ChatId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<RoleRankModel, RoleModel>()
                .ForMember(dest => dest.RoleName, opt => opt.Ignore())
                .ForMember(dest => dest.RoleColor, opt => opt.Ignore())
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<Message, MessageModel>()
                .ForMember(dest => dest.FileModels, opt => opt.MapFrom(d => d.Files))
                .ReverseMap();
            
            CreateMap<Reaction, ReactionModel>()
                .ReverseMap();
            
            CreateMap<FileModel, FileEntity>()
                .ReverseMap();
            
            CreateMap<FileViewModel, FileModel>()
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<FileSend, FileModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<MessageViewModel, MessageModel>()
                .ForMember(dest => dest.FileModels, opt => opt.MapFrom(d => d.FileModels))
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.ToReplyMessage, opt => opt.Ignore())
                .ForMember(dest => dest.ChatModel, opt => opt.Ignore())
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src.Reactions))
                .ReverseMap();
            
            CreateMap<ReactionViewModel, ReactionModel>()
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<AddReactionModel, ReactionModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.MessageId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserViewModel, ChatMemberModel>()
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ReverseMap();

        }
    }
}
