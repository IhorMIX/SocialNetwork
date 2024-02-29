using System.Collections.Concurrent;

namespace SocialNetwork.Web.Helpers;

public class UserInChatTracker : IUserInChatTracker
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _groupUsers = new ConcurrentDictionary<string, HashSet<string>>();

    public void AddToGroup(string userId, string groupName)
    {
        _groupUsers.AddOrUpdate(groupName,
            (key) => new HashSet<string> { userId },
            (key, set) =>
            {
                set.Add(userId);
                return set;
            });
    }

    public void RemoveFromGroup(string userId, string groupName)
    {
        if (_groupUsers.TryGetValue(groupName, out var users))
        {
            users.Remove(userId);
        }
    }

    public List<string> GetUsersInGroup(string groupName)
    {
        return _groupUsers.TryGetValue(groupName, out var users) ? new List<string>(users) : new List<string>();
    }
}