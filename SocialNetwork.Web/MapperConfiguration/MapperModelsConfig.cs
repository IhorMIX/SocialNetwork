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
            
            CreateMap<UserModel, User>()
                .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfoId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AuthorizationInfoModel, AuthorizationInfo>().ReverseMap();
            CreateMap<ProfileModel, SocialNetwork.DAL.Entity.Profile>().ReverseMap();
        }
    }
}
