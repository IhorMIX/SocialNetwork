namespace SocialNetwork.BLL.Exceptions;

public class CommentNotFoundException : CustomException
{
    public CommentNotFoundException(string message) : base(message)
    {
    }
}