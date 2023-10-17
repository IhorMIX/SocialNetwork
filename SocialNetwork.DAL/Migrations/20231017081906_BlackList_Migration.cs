using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BlackList_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlackLists",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BannedUserId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackLists", x => new { x.UserId, x.BannedUserId });
                    table.ForeignKey(
                        name: "FK_BlackLists_Users_BannedUserId",
                        column: x => x.BannedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlackLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlackLists_BannedUserId",
                table: "BlackLists",
                column: "BannedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackLists");
        }
    }
}
