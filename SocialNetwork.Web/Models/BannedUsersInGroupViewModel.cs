using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.Web.Models
{
    public class BannedUsersInGroupViewModel
    {
        public int Id { get; set; }
        public BannedUserViewModel User { get; set; } = null!;
        public string Reason { get; set; } = null!;
    }
}
