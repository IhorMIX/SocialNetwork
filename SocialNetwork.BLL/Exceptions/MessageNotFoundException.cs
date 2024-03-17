namespace SocialNetwork.BLL.Exceptions;

public class MessageNotFoundException: CustomException
{
    public MessageNotFoundException(string message) : base(message)
    {
    }
}