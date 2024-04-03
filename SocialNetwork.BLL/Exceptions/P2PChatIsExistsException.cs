namespace SocialNetwork.BLL.Exceptions;

public class P2PChatIsExistsException: CustomException
{
    public P2PChatIsExistsException(string message) : base(message)
    {
    }
}