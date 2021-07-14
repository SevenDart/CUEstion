using Microsoft.EntityFrameworkCore.Migrations;

namespace CUEstion.DAL.Migrations
{
    public partial class elementMarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerMarks",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    Mark = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerMarks", x => new { x.UserId, x.AnswerId });
                    table.ForeignKey(
                        name: "FK_AnswerMarks_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerMarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentMarks",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    Mark = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentMarks", x => new { x.UserId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_CommentMarks_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentMarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionMarks",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Mark = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionMarks", x => new { x.UserId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_QuestionMarks_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionMarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerMarks_AnswerId",
                table: "AnswerMarks",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMarks_CommentId",
                table: "CommentMarks",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionMarks_QuestionId",
                table: "QuestionMarks",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerMarks");

            migrationBuilder.DropTable(
                name: "CommentMarks");

            migrationBuilder.DropTable(
                name: "QuestionMarks");
        }
    }
}
