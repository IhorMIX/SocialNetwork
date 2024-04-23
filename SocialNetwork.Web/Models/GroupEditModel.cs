namespace SocialNetwork.Web.Models
{
    public class GroupEditModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsPrivate { get; set; } 
        public string Logo { get; set; } = null!;
    }
}