using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.BLL.Models;

public class ProfileModel : BaseModel
{
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string AvatarImage { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Sex Sex { get; set; }
    
}