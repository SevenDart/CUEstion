using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class marksInheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "QuestionMarks",
                newName: "MarkValue");

            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "CommentMarks",
                newName: "MarkValue");

            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "AnswerMarks",
                newName: "MarkValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MarkValue",
                table: "QuestionMarks",
                newName: "Mark");

            migrationBuilder.RenameColumn(
                name: "MarkValue",
                table: "CommentMarks",
                newName: "Mark");

            migrationBuilder.RenameColumn(
                name: "MarkValue",
                table: "AnswerMarks",
                newName: "Mark");
        }
    }
}
