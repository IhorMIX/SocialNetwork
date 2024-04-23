using SocialNetwork.BLL.Models.Enums;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.BLL.Helpers;

public static class AccessHelper
{
    public static bool HasAccess(this ICollection<Role> roles, List<ChatAccess> accesses)
    {
        return accesses.All(access => roles.Any(role => role.RoleAccesses.Any(i => i.ChatAccess == access)));
    }
    public static bool HasAccess(this ICollection<RoleGroup> roles, List<GroupAccess> accesses)
    {
        return accesses.All(access => roles.Any(role => role.RoleAccesses.Any(i => i.GroupAccess == access)));
    }
}