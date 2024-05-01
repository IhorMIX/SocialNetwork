namespace SocialNetwork.Web.Models
{
    public class GroupCreateViewModel
    {
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string Description { get; set; } = null!;
        public string Logo { get; set; } = null!;
    }
}