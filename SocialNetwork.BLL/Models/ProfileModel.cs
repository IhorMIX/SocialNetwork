using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.BLL.Models;

public class ProfileModel : BaseModel
{
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public DateTime? Birthday { get; set; }

    public string AvatarImage { get; set; }

    public string Description { get; set; }

    public Sex Sex { get; set; }
    
}