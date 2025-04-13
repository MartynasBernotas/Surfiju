using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTripEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Trips",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Trips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Trips",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerId",
                table: "Trips",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Trips",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_OrganizerId",
                table: "Trips",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_AspNetUsers_OrganizerId",
                table: "Trips",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_AspNetUsers_OrganizerId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_OrganizerId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "OrganizerId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Trips");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Trips",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
