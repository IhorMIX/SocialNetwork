using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class Group : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string Logo { get; set; } = null!;
        public ICollection<RoleGroup> RoleGroups { get; set; } = null!;
        public ICollection<GroupMember> GroupMembers { get; set; } = null!;
        public ICollection<BannedUserList> BannedUsers { get; set; } = null!;
    }
}
