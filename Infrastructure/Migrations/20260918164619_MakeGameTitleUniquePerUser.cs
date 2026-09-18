using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeGameTitleUniquePerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Games_Title",
                table: "Games");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Title_UserId",
                table: "Games",
                columns: new[] { "Title", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Games_Title_UserId",
                table: "Games");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Title",
                table: "Games",
                column: "Title",
                unique: true);
        }
    }
}
