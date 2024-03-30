namespace SocialNetwork.Web.Models
{
    public class ForRoleGroupModel
    {
        public int GroupId { get; set; }
        public int RoleId { get; set; }
        public List<int>? MemberIds { get; set; }
    }
}