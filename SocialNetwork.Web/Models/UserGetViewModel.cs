using SocialNetwork.BL.Models;

namespace SocialNetwork.Web.Models
{
    public class UserGetViewModel : BaseModel
    {
        public ProfileGetViewModel Profile { get; set; }
    }
}
