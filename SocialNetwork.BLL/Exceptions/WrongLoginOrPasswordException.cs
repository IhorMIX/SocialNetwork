namespace SocialNetwork.BLL.Exceptions;

public class WrongLoginOrPasswordException : CustomException
{
    public WrongLoginOrPasswordException(string message) : base(message)
    {
    }
}