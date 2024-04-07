namespace SocialNetwork.Web.Models;

public class UserPostViewModel : BasePostViewModel
{
    public ICollection<FileViewModel> Files { get; set; } = null!;
    public UserViewModel User { get; set; } = null!;
}