using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Like_And_Comment_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentPostId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LikePostId",
                table: "Notifications",
                type: "int",
                nullable: true);
            
            migrationBuilder.CreateTable(
                name: "CommentPost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToReplyCommentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentPost_CommentPost_ToReplyCommentId",
                        column: x => x.ToReplyCommentId,
                        principalTable: "CommentPost",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentPost_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentPost_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LikePost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikePost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikePost_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikePost_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentPostId",
                table: "Notifications",
                column: "CommentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LikePostId",
                table: "Notifications",
                column: "LikePostId");
            
            migrationBuilder.CreateIndex(
                name: "IX_CommentPost_PostId",
                table: "CommentPost",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentPost_ToReplyCommentId",
                table: "CommentPost",
                column: "ToReplyCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentPost_UserId",
                table: "CommentPost",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LikePost_PostId",
                table: "LikePost",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_LikePost_UserId",
                table: "LikePost",
                column: "UserId");
            
            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_CommentPost_CommentPostId",
                table: "Notifications",
                column: "CommentPostId",
                principalTable: "CommentPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_LikePost_LikePostId",
                table: "Notifications",
                column: "LikePostId",
                principalTable: "LikePost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_CommentPost_CommentPostId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_LikePost_LikePostId",
                table: "Notifications");
            
            migrationBuilder.DropTable(
                name: "CommentPost");

            migrationBuilder.DropTable(
                name: "LikePost");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CommentPostId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_LikePostId",
                table: "Notifications");
            
            migrationBuilder.DropColumn(
                name: "CommentPostId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LikePostId",
                table: "Notifications");
        }
    }
}
