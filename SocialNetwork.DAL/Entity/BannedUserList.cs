using System.Text.RegularExpressions;

namespace SocialNetwork.DAL.Entity
{
    public class BannedUserList : BaseEntity
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
        public string Reason { get; set; } = null!;
    }
}