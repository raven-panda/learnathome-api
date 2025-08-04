using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnAtHomeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMentorToRpUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "Users",
                type: "INTEGER",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "StudentTasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_MentorId",
                table: "Users",
                column: "MentorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_MentorId",
                table: "StudentTasks",
                column: "MentorId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTasks_Users_MentorId",
                table: "StudentTasks",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_MentorId",
                table: "Users",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTasks_Users_MentorId",
                table: "StudentTasks"
            );

            migrationBuilder.DropForeignKey(name: "FK_Users_Users_MentorId", table: "Users");

            migrationBuilder.DropIndex(name: "IX_Users_MentorId", table: "Users");

            migrationBuilder.DropIndex(name: "IX_StudentTasks_MentorId", table: "StudentTasks");

            migrationBuilder.DropColumn(name: "MentorId", table: "Users");

            migrationBuilder.DropColumn(name: "MentorId", table: "StudentTasks");
        }
    }
}
