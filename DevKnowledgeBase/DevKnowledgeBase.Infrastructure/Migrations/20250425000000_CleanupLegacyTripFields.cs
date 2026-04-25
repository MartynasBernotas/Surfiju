using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanupLegacyTripFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TripId",
                table: "CampMembers");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "Expenses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TripId",
                table: "CampMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TripId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
