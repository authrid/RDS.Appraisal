using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppraisalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedSessionJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SavedSessionJson",
                table: "Appraisals",
                type: "TEXT",
                maxLength: 16000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedSessionJson",
                table: "Appraisals");
        }
    }
}
