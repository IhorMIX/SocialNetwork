namespace SocialNetwork.Web.Models
{
    public class ProfileViewModel
    {
        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime Birthday { get; set; }

        public string AvatarImage { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Sex { get; set; } = null!;
    }
}
