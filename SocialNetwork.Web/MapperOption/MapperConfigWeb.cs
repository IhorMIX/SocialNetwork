using AutoMapper;
using SocialNetwork.BL.Models;
using SocialNetwork.DAL.Entity;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.MapperOption
{
    public class MapperConfigWeb
    {
        public static Mapper InitializeAutomapper()
        {

            var config = new MapperConfiguration(cfg =>      
            {
                //Configuring Employee and EmployeeDTO
                cfg.CreateMap<UserCreateViewModel, UserModel>();
            });

            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
