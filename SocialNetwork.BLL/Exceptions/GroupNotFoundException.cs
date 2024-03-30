using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Exceptions
{
    internal class GroupNotFoundException : CustomException
    {
        public GroupNotFoundException(string message) : base(message)
        {
        }
    }
}