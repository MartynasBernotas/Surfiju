using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotosUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Trips",
                newName: "IsPublic");

            migrationBuilder.AddColumn<List<string>>(
                name: "PhotoUrls",
                table: "Trips",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrls",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "Trips",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Trips",
                type: "text",
                nullable: true);
        }
    }
}
