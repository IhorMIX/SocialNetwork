namespace SocialNetwork.BLL.Models
{
    public class GroupModel : BaseModel
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string Logo { get; set; } = null!;

        public ICollection<GroupMemberModel>? GroupMembers { get; set; }

        public ICollection<RoleGroupModel>? Roles { get; set; }
    }
}