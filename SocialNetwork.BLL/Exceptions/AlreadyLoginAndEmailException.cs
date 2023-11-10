namespace SocialNetwork.BLL.Exceptions;

public class AlreadyLoginAndEmailException : CustomException
{
    public AlreadyLoginAndEmailException(string message) : base(message)
    {

    }
}
