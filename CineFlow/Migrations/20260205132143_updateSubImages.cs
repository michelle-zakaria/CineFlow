using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineFlow.Migrations
{
    /// <inheritdoc />
    public partial class updateSubImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubImageURL",
                schema: "admin",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "MovieSubImages",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSubImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSubImages_Movies_MovieId",
                        column: x => x.MovieId,
                        principalSchema: "admin",
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_EndDate",
                schema: "admin",
                table: "Movies",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Price",
                schema: "admin",
                table: "Movies",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_StartDate",
                schema: "admin",
                table: "Movies",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                schema: "admin",
                table: "Movies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSubImages_DisplayOrder",
                schema: "admin",
                table: "MovieSubImages",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSubImages_MovieId",
                schema: "admin",
                table: "MovieSubImages",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSubImages_MovieId_DisplayOrder",
                schema: "admin",
                table: "MovieSubImages",
                columns: new[] { "MovieId", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieSubImages",
                schema: "admin");

            migrationBuilder.DropIndex(
                name: "IX_Movies_EndDate",
                schema: "admin",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Price",
                schema: "admin",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_StartDate",
                schema: "admin",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_Title",
                schema: "admin",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "SubImageURL",
                schema: "admin",
                table: "Movies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
