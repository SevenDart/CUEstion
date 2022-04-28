using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class workspaceAdditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChiefId",
                table: "Workspaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Workspaces",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageRoles",
                table: "WorkspaceRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_ChiefId",
                table: "Workspaces",
                column: "ChiefId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserId",
                table: "Workspaces",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_ChiefId",
                table: "Workspaces",
                column: "ChiefId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_UserId",
                table: "Workspaces",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_ChiefId",
                table: "Workspaces");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_UserId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_ChiefId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_UserId",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "ChiefId",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "CanManageRoles",
                table: "WorkspaceRoles");
        }
    }
}
