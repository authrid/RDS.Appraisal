using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppraisalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOcrResultsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OcrResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppraisalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Province = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubDistrict = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    JenisSertifikat = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NomorSertifikat = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NamaPemegang = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Nib = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LuasTanah = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LuasBangunan = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcrResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcrResults_Appraisals_AppraisalId",
                        column: x => x.AppraisalId,
                        principalTable: "Appraisals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OcrResults_AppraisalId",
                table: "OcrResults",
                column: "AppraisalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OcrResults");
        }
    }
}
