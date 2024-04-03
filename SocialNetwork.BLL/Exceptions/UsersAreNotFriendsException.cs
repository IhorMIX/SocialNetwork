namespace SocialNetwork.BLL.Exceptions;

public class UsersAreNotFriendsException: CustomException
{
    public UsersAreNotFriendsException(string message) : base(message)
    {
    }
}