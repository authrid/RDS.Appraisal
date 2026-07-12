using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppraisalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedPropertyListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedPropertyListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppraisalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Price = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Date = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Lt = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Lb = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Kt = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Km = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DetailDescription = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Certificate = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Floor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Electricity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Furnished = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Facing = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LocationText = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Transaction = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PropertyType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AddressDetail = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    LocationDetail = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    GroupDetail = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Garage = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ListedDate = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IdListing = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPropertyListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedPropertyListings_Appraisals_AppraisalId",
                        column: x => x.AppraisalId,
                        principalTable: "Appraisals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedPropertyListings_AppraisalId",
                table: "SavedPropertyListings",
                column: "AppraisalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedPropertyListings");
        }
    }
}
