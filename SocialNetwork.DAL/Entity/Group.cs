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

        public string Logo { get; set; } = null!;
        //public bool IsPrivate { get; set; }
        public ICollection<RoleGroup>? RoleGroups { get; set; }
        public ICollection<GroupMember>? GroupMembers { get; set; }
        public ICollection<BannedUserList>? BannedUsers { get; set; }
    }
}
