using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Entity.Enums
{
    public enum GroupAccess
    {
        CreatePost,
        DeletedPost,
        EditRoles,
        InviteToGroupMembers,
        DelGroupMembers,
        BanGroupMembers,
        MuteGroupMembers,
        DelMessages,
        EditNicknames,
        EditGroup
    }
}
