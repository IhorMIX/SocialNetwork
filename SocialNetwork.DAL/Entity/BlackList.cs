namespace SocialNetwork.DAL.Entity
{
    public class BlackList : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BannedUserId { get; set; }
        public User BannedUser { get; set; } = null!;
    }
}