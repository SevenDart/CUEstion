using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class workspaceChiefFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_UserId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_UserId",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Workspaces");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Workspaces",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserId",
                table: "Workspaces",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_UserId",
                table: "Workspaces",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
