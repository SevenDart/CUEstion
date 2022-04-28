using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class workspacesFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanAddUsers = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceRoles_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false),
                    WorkspaceRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceUsers", x => new { x.UserId, x.WorkspaceId });
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_WorkspaceRoles_WorkspaceRoleId",
                        column: x => x.WorkspaceRoleId,
                        principalTable: "WorkspaceRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_WorkspaceId",
                table: "Questions",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceRoles_WorkspaceId",
                table: "WorkspaceRoles",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_WorkspaceId",
                table: "WorkspaceUsers",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_WorkspaceRoleId",
                table: "WorkspaceUsers",
                column: "WorkspaceRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Workspaces_WorkspaceId",
                table: "Questions",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Workspaces_WorkspaceId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "WorkspaceUsers");

            migrationBuilder.DropTable(
                name: "WorkspaceRoles");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Questions_WorkspaceId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                table: "Questions");
        }
    }
}
