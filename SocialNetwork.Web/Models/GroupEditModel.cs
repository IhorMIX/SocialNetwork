namespace SocialNetwork.Web.Models
{
    public class GroupEditModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;

        public string Logo { get; set; } = null!;
    }
}
