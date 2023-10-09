using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Message_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddMembers",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DelMembers",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DelMessages",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EditChat",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EditNicknames",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EditRoles",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "MuteMembers",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "SendAudioMess",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "SendFiles",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "SendMessages",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "RoleAccesses",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    ToReplyMessageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_ChatMembers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "ChatMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_ToReplyMessageId",
                        column: x => x.ToReplyMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_ChatMembers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "ChatMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reactions_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AuthorId",
                table: "Messages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ToReplyMessageId",
                table: "Messages",
                column: "ToReplyMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_AuthorId",
                table: "Reactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_MessageId",
                table: "Reactions",
                column: "MessageId");

            migrationBuilder.Sql(
                $"INSERT INTO Roles VALUES ('Admin', '#FF0000', NULL, 0, 'SendMessages,SendAudioMess,SendFiles,EditRoles,AddMembers,DelMembers,MuteMembers,DelMessages,EditNicknames,EditChat')");
            
            migrationBuilder.Sql(
                $"INSERT INTO Roles VALUES ('P2PAdmin', '#FF0000', NULL, 0, 'SendMessages,SendAudioMess,SendFiles,MuteMembers,DelMessages')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropColumn(
                name: "RoleAccesses",
                table: "Roles");

            migrationBuilder.AddColumn<bool>(
                name: "AddMembers",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DelMembers",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DelMessages",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditChat",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditNicknames",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditRoles",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MuteMembers",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendAudioMess",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendFiles",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendMessages",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
