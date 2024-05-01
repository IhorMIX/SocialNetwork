using SocialNetwork.DAL.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class RoleGroupAccess : BaseEntity
    {
        public GroupAccess GroupAccess { get; set; }

        public int RoleId { get; set; }

        public RoleGroup RoleGroup { get; set; } = null!;
    }
}