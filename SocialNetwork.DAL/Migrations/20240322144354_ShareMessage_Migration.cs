using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ShareMessage_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatMembers_AuthorId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleChatAccess_Roles_RoleId",
                table: "RoleChatAccess");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_AuthorId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatorId",
                table: "Messages",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatMembers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "ChatMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_CreatorId",
                table: "Messages",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleChatAccess_Roles_RoleId",
                table: "RoleChatAccess",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatMembers_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_CreatorId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleChatAccess_Roles_RoleId",
                table: "RoleChatAccess");

            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatorId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                newName: "IX_Messages_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatMembers_AuthorId",
                table: "Messages",
                column: "AuthorId",
                principalTable: "ChatMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleChatAccess_Roles_RoleId",
                table: "RoleChatAccess",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
