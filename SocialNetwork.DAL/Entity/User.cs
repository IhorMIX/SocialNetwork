using Azure.Core;
using SocialNetwork.DAL.Entity.Enums;

namespace SocialNetwork.DAL.Entity;

public class User : BaseEntity
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public OnlineStatus OnlineStatus { get; set; }

    public bool IsEnabled { get; set; }

    public int ProfileId { get; set; }

    public int AuthorizationInfoId { get; set; }

    public Profile Profile { get; set; } = null!;

    public AuthorizationInfo? AuthorizationInfo { get; set; }

    public IEnumerable<Friendship>? Friends { get; set; }

    public IEnumerable<BaseRequestEntity>? Requests { get; set; }

    public IEnumerable<ChatMember>? ChatMembers { get; set; }
    public IEnumerable<BlackList>? BlackLists { get; set; }
    
    public IEnumerable<BaseNotificationEntity>? Notifications { get; set; }
    public IEnumerable<GroupMember>? GroupMembers { get; set; }
    public IEnumerable<BannedUserList>? BansByGroups { get; set; }
    public ICollection<Message>? CreatedMessages { get; set; }
}