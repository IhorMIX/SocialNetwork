using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Models
{
    public class BaseRequestModel : BaseModel
    {
        public DateTime CreatedAt { get; set; }
        public int SenderId { get; set; }
        public UserModel Sender { get; set; } = null!;
    }
}
