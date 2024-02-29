namespace SocialNetwork.BLL.Exceptions;

public class NotificationNotFoundException: CustomException
{
    public NotificationNotFoundException(string message) : base(message)
    {
    }
}