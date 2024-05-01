using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Models
{
    public class GroupMemberModel : BaseModel
    {
        public GroupModel Group { get; set; } = null!;
        public UserModel User { get; set; } = null!;
        public ICollection<RoleGroupModel> RoleGroup { get; set; } = null!;
    }
}
