namespace SocialNetwork.Web.Models;

public class PostViewModel
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ICollection<FileViewModel> Files { get; set; } = null!;
    public UserViewModel User { get; set; } = null!;
}