namespace SocialNetwork.Web.Models
{
    public class RoleGroupUpdateModel
    {
        public int GroupId { get; set; }
        public int RoleId { get; set; }
        public RoleGroupViewModel RoleGroupModel { get; set; } = null!;
    }
}
