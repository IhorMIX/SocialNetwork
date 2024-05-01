using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class GroupRequest : BaseRequestEntity
    {
        public int ToGroupId { get; set; }
        public Group ToGroup { get; set; } = null!;
    }
}
