namespace SocialNetwork.BLL.Exceptions;

public class PostNotFoundException : CustomException
{
    public PostNotFoundException(string message) : base(message)
    {
    }
}