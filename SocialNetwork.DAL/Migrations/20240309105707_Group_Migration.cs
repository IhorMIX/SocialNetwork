using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Group_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannedUserLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedUserLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannedUserLists_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannedUserLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsCreator = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMemberRoleGroup",
                columns: table => new
                {
                    GroupMembersId = table.Column<int>(type: "int", nullable: false),
                    RoleGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMemberRoleGroup", x => new { x.GroupMembersId, x.RoleGroupId });
                    table.ForeignKey(
                        name: "FK_GroupMemberRoleGroup_GroupMembers_GroupMembersId",
                        column: x => x.GroupMembersId,
                        principalTable: "GroupMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMemberRoleGroup_RoleGroups_RoleGroupId",
                        column: x => x.RoleGroupId,
                        principalTable: "RoleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleGroupAccess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupAccess = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleGroupAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleGroupAccess_RoleGroups_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BannedUserLists_GroupId",
                table: "BannedUserLists",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUserLists_UserId",
                table: "BannedUserLists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberRoleGroup_RoleGroupId",
                table: "GroupMemberRoleGroup",
                column: "RoleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_UserId",
                table: "GroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleGroupAccess_RoleId",
                table: "RoleGroupAccess",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleGroups_GroupId",
                table: "RoleGroups",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannedUserLists");

            migrationBuilder.DropTable(
                name: "GroupMemberRoleGroup");

            migrationBuilder.DropTable(
                name: "RoleGroupAccess");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "RoleGroups");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
