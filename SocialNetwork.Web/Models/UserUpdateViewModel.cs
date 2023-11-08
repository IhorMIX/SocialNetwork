using SocialNetwork.BLL.Models;

namespace SocialNetwork.Web.Models
{
    public class UserUpdateViewModel
    {
        public string? Password { get; set; }

        public ProfileUpdateViewModel Profile { get; set; }
    }
}
