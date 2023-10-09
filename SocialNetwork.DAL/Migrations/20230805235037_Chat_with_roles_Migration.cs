using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Chat_with_roles_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsGroup = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMembers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendMessages = table.Column<bool>(type: "bit", nullable: false),
                    SendAudioMess = table.Column<bool>(type: "bit", nullable: false),
                    SendFiles = table.Column<bool>(type: "bit", nullable: false),
                    EditRoles = table.Column<bool>(type: "bit", nullable: false),
                    AddMembers = table.Column<bool>(type: "bit", nullable: false),
                    DelMembers = table.Column<bool>(type: "bit", nullable: false),
                    MuteMembers = table.Column<bool>(type: "bit", nullable: false),
                    DelMessages = table.Column<bool>(type: "bit", nullable: false),
                    EditNicknames = table.Column<bool>(type: "bit", nullable: false),
                    EditChat = table.Column<bool>(type: "bit", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMemberRole",
                columns: table => new
                {
                    ChatMembersId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMemberRole", x => new { x.ChatMembersId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ChatMemberRole_ChatMembers_ChatMembersId",
                        column: x => x.ChatMembersId,
                        principalTable: "ChatMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMemberRole_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMemberRole_RoleId",
                table: "ChatMemberRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_ChatId",
                table: "ChatMembers",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_UserId",
                table: "ChatMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ChatId",
                table: "Roles",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMemberRole");

            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
