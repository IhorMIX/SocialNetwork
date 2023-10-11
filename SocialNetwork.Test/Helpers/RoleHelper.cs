using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.DAL;
using SocialNetwork.DAL.Entity;
using SocialNetwork.DAL.Entity.Enums;
using SocialNetwork.DAL.Repository.Interfaces;

namespace SocialNetwork.Test.Helpers;

public static class RoleHelper
{
    public static List<Role> CreateRole()
    {
        
        var roleAdmin = new Role
        {
            RoleName = "Admin",
            RoleColor = "#FFFFFF",
            RoleAccesses = new List<RoleChatAccess>()
            {
                new()
                {
                    ChatAccess = ChatAccess.SendMessages
                },
                new()
                {
                    ChatAccess = ChatAccess.SendAudioMess
                },
                new()
                {
                    ChatAccess = ChatAccess.SendFiles
                },
                new()
                {
                    ChatAccess = ChatAccess.EditRoles
                },
                new()
                {
                    ChatAccess = ChatAccess.AddMembers
                },
                new()
                {
                    ChatAccess = ChatAccess.DelMembers
                },
                new()
                {
                    ChatAccess = ChatAccess.MuteMembers
                },
                new()
                {
                    ChatAccess = ChatAccess.DelMessages
                },
                new()
                {
                    ChatAccess = ChatAccess.EditNicknames
                },
                new()
                {
                    ChatAccess = ChatAccess.EditChat
                },
            },
            Rank = 100000
        };
        var p2pAdmin = new Role
        {
            RoleName = "P2PAdmin",
            RoleColor = "null",
            RoleAccesses = new List<RoleChatAccess>()
            {
                new()
                {
                    ChatAccess = ChatAccess.SendMessages
                },
                new()
                {
                    ChatAccess = ChatAccess.SendAudioMess
                },
                new()
                {
                    ChatAccess = ChatAccess.SendFiles
                },
                new()
                {
                    ChatAccess = ChatAccess.MuteMembers
                },
                new()
                {
                    ChatAccess = ChatAccess.DelMessages
                },
                new()
                {
                    ChatAccess = ChatAccess.EditChat
                },
            },
        };

        return new List<Role>()
        {
            roleAdmin,
            p2pAdmin
        };
    }
}