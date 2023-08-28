namespace SocialNetwork.Web.Models;

public class RoleViewModel
{
    public int Id { get; set; }
    public string RoleName { get; set; }
    public string RoleColor { get; set; }

    public bool SendMessages { get; set; }
    public bool SendAudioMess { get; set; }
    public bool SendFiles { get; set; }
    
    public bool EditRoles { get; set; }
    public bool AddMembers { get; set; }
    public bool DelMembers { get; set; }
    public bool MuteMembers { get; set; }
    public bool DelMessages { get; set; }
    public bool EditNicknames { get; set; }
    public bool EditChat { get; set; }
    
    public ICollection<int> UsersIds { get; set; }
    
    public int Rank { get; set; }
}