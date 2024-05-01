using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BaseRequest_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_Users_ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_Users_SenderId",
                table: "FriendRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                newName: "Requests");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_SenderId",
                table: "Requests",
                newName: "IX_Requests_SenderId");

            migrationBuilder.AddColumn<int>(
                name: "GroupRequestId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Requests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ToGroupId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToUserId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ToGroupId",
                table: "Requests",
                column: "ToGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ToUserId",
                table: "Requests",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Groups_ToGroupId",
                table: "Requests",
                column: "ToGroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_SenderId",
                table: "Requests",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_ToUserId",
                table: "Requests",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Groups_ToGroupId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_SenderId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_ToUserId",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ToGroupId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ToUserId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "GroupRequestId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ToGroupId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ToUserId",
                table: "Requests");

            migrationBuilder.RenameTable(
                name: "Requests",
                newName: "FriendRequests");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_SenderId",
                table: "FriendRequests",
                newName: "IX_FriendRequests_SenderId");

            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "FriendRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests",
                columns: new[] { "Id", "SenderId", "ReceiverId" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_Users_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_Users_SenderId",
                table: "FriendRequests",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
