using SocialNetwork.DAL.Entity;

namespace SocialNetwork.BLL.Models
{
    public class BlackListModel
    {
        public int UserId { get; set; }
        public UserModel? User { get; set; }

        public int BannedUserId { get; set; }
        public UserModel? BannedUser { get; set; }
    }
}