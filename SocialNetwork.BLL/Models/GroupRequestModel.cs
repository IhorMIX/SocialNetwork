using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Models
{
    public class GroupRequestModel : BaseRequestModel
    {
        public int ToGroupId { get; set; }
        public Group ToGroup { get; set; } = null!;
    }
}
