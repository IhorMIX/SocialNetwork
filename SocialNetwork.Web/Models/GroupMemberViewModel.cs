namespace SocialNetwork.Web.Models
{
    public class GroupMemberViewModel
    {
        public int Id { get; set; }
        public UserViewModel User { get; set; } = null!;
        public ICollection<GroupMemberRoleGroupViewModel> RoleGroup { get; set; } = null!;
    }
}