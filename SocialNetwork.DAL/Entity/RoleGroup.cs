using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class RoleGroup : BaseEntity
    {
        public int GroupId { get; set; }
        public string RoleName { get; set; } = null!;
        public string RoleColor { get; set; } = null!;

        public ICollection<RoleGroupAccess> RoleAccesses { get; set; } = null!;

        public ICollection<GroupMember> GroupMembers { get; set; } = null!;
        public Group Group { get; set; } = null!;
        public int Rank { get; set; }
    }
}
