using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity
{
    public class BaseRequestEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;
    }
}