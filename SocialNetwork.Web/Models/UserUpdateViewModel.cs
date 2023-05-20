using SocialNetwork.BL.Models;

namespace SocialNetwork.Web.Models
{
    public class UserUpdateViewModel:BaseModel
    {
        public string? Login { get; set; }

        public string? Password { get; set; }

        public ProfileUpdateViewModel Profile { get; set; }
    }
}
