using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;
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

            CreateMap<UserModel, User>()
                .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfoId, opt => opt.Ignore())
                .ReverseMap();
            
            CreateMap<AuthorizationInfoModel, AuthorizationInfo>().ReverseMap();
            CreateMap<ProfileModel, SocialNetwork.DAL.Entity.Profile>().ReverseMap();
        }
    }
}
