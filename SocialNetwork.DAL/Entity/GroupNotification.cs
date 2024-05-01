using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class GroupNotification : NotificationEntity
    {
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
    }
}
