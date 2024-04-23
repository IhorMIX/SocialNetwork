using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Exceptions
{
    public class RequestException : CustomException
    {
        public RequestException(string message) : base(message)
        {
        }
    }
}
