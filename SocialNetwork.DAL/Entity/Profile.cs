using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class Profile : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string AvatarImage { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Sex Sex { get; set; }

    public int UserId { get; set; }
    
    public User User { get; set; } = null!;
}