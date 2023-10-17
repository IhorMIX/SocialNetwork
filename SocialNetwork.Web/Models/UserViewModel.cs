using SocialNetwork.BL.Models;

namespace SocialNetwork.Web.Models
{
    public class UserViewModel
    {
        private int Id { get; set; }
        public ProfileViewModel Profile { get; set; }
    }
}
