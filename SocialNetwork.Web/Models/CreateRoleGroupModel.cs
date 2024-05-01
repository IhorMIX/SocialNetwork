using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.Web.Models
{
    public class CreateRoleGroupModel
    {
        public int GroupId { get; set; }
        public string RoleName { get; set; } = null!;

        public string RoleColor { get; set; } = null!;

        public List<GroupAccess> RoleAccesses { get; set; } = new();
        public int Rank { get; set; }
    }
}