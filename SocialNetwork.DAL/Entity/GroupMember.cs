using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class GroupMember : BaseEntity
    {
        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<RoleGroup> RoleGroup { get; set; } = null!;
        public bool IsCreator { get; set; }
    }
}