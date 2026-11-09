using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueGameTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Games_Title",
                table: "Games",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Games_Title",
                table: "Games");
        }
    }
}
