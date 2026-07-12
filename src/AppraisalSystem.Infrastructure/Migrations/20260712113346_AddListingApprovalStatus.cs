using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppraisalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "SavedPropertyListings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "SavedPropertyListings");
        }
    }
}
