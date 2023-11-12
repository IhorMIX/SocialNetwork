namespace SocialNetwork.BLL.Exceptions;

public class CreatorCantLeaveException: Exception
{
    public CreatorCantLeaveException(string message) : base(message)
    {
    }
}