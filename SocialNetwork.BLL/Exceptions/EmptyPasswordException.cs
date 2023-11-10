namespace SocialNetwork.BLL.Exceptions;

public class EmptyPasswordException: CustomException
{
    public EmptyPasswordException(string message) : base(message)
    {
    }
}