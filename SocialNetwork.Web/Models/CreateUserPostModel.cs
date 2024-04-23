
namespace SocialNetwork.Web.Models;

public class CreateUserPostModel
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public ICollection<FileSend> FilePaths { get; set; } = null!;
}