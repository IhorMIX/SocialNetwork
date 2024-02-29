namespace SocialNetwork.Web.Helpers;

public interface IUserInChatTracker
{
    void AddToGroup(string userId, string groupName);
    void RemoveFromGroup(string userId, string groupName);
    List<string> GetUsersInGroup(string groupName);
}