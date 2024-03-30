using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Models
{
    public class BannedUserInGroupModel : BaseModel
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public GroupModel Group { get; set; } = null!;
        public UserModel User { get; set; } = null!;
        public string Reason { get; set; } = null!;
    }
}
