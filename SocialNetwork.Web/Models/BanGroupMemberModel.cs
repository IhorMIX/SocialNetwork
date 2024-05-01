namespace SocialNetwork.Web.Models
{
    public class BanGroupMemberModel
    {
        public int GroupId { get; set; }
        public int BannedGroupMemberId { get; set; }
        public string Reason { get; set; } = null!;
    }
}
