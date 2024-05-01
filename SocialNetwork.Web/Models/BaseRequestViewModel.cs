using SocialNetwork.BLL.Models;

namespace SocialNetwork.Web.Models
{
    public class BaseRequestViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserViewModel Sender { get; set; } = null!;
    }
}
