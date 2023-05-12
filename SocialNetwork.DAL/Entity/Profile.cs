using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class Profile : BaseEntity
{
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public DateTime Birthday { get; set; }

    public string AvatarImage { get; set; }

    public string Description { get; set; }

    public Sex Sex { get; set; }

    public int UserId { get; set; }
    
    public User User { get; set; }
}