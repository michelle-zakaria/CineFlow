using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineFlow.Migrations
{
    /// <inheritdoc />
    public partial class updateBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "admins");

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShowTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumberOfTickets = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SeatNumbers = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SeatType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Regular"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Confirmed"),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalSchema: "admin",
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingSeats",
                schema: "admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Row = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SeatIndex = table.Column<int>(type: "int", nullable: false),
                    SeatType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Regular"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingSeats_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "admins",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingDate",
                schema: "admins",
                table: "Bookings",
                column: "BookingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingNumber",
                schema: "admins",
                table: "Bookings",
                column: "BookingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerEmail",
                schema: "admins",
                table: "Bookings",
                column: "CustomerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MovieId",
                schema: "admins",
                table: "Bookings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MovieId_ShowDate_ShowTime",
                schema: "admins",
                table: "Bookings",
                columns: new[] { "MovieId", "ShowDate", "ShowTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ShowDate",
                schema: "admins",
                table: "Bookings",
                column: "ShowDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                schema: "admins",
                table: "Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_BookingId",
                schema: "admins",
                table: "BookingSeats",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_BookingId_SeatNumber",
                schema: "admins",
                table: "BookingSeats",
                columns: new[] { "BookingId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_SeatNumber",
                schema: "admins",
                table: "BookingSeats",
                column: "SeatNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingSeats",
                schema: "admins");

            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "admins");
        }
    }
}
