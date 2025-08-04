using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnAtHomeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationsToStudentTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "StudentTasks",
                newName: "UpdatedById"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "StudentTasks",
                newName: "CreatedById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_CreatedById",
                table: "StudentTasks",
                column: "CreatedById"
            );

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_UpdatedById",
                table: "StudentTasks",
                column: "UpdatedById"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTasks_Users_CreatedById",
                table: "StudentTasks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTasks_Users_UpdatedById",
                table: "StudentTasks",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTasks_Users_CreatedById",
                table: "StudentTasks"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTasks_Users_UpdatedById",
                table: "StudentTasks"
            );

            migrationBuilder.DropIndex(name: "IX_StudentTasks_CreatedById", table: "StudentTasks");

            migrationBuilder.DropIndex(name: "IX_StudentTasks_UpdatedById", table: "StudentTasks");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "StudentTasks",
                newName: "UpdatedByUserId"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "StudentTasks",
                newName: "CreatedByUserId"
            );
        }
    }
}
