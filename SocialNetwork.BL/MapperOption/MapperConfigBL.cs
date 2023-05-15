using AutoMapper;
using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BL.MapperOption
{
    public class MapperConfigBL
    {
        public static Mapper InitializeAutomapper()
        {

            var config = new MapperConfiguration(cfg =>
            {
                //Configuring Employee and EmployeeDTO
                cfg.CreateMap<User, UserModel>();
                cfg.CreateMap<UserModel, User>()
                .ForMember(dest => dest.ProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationInfoId, opt => opt.Ignore());
            });

            var mapper = new Mapper(config);
            return mapper;
        }
    }
}                      
