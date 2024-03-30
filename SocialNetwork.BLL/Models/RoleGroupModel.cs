using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Models
{
    public class RoleGroupModel : BaseModel
    {
        public string RoleName { get; set; } = null!;
        public string RoleColor { get; set; } = null!;

        public List<GroupAccess> RoleAccesses { get; set; } = new();

        public ICollection<int> UsersIds { get; set; } = null!;
        public GroupModel? Group { get; set; }
        public int Rank { get; set; }
    }
}