using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamingToCamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Trips_TripId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "TripMembers");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TripId",
                table: "Expenses");

            migrationBuilder.AddColumn<Guid>(
                name: "CampId",
                table: "Expenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Camps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    PhotoUrls = table.Column<List<string>>(type: "text[]", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    OrganizerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Camps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Camps_AspNetUsers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampMembers_Camps_CampId",
                        column: x => x.CampId,
                        principalTable: "Camps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CampId",
                table: "Expenses",
                column: "CampId");

            migrationBuilder.CreateIndex(
                name: "IX_CampMembers_CampId",
                table: "CampMembers",
                column: "CampId");

            migrationBuilder.CreateIndex(
                name: "IX_CampMembers_UserId",
                table: "CampMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Camps_OrganizerId",
                table: "Camps",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Camps_CampId",
                table: "Expenses",
                column: "CampId",
                principalTable: "Camps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Camps_CampId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "CampMembers");

            migrationBuilder.DropTable(
                name: "Camps");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CampId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CampId",
                table: "Expenses");

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizerId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhotoUrls = table.Column<List<string>>(type: "text[]", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_AspNetUsers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripMembers_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TripId",
                table: "Expenses",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_TripMembers_TripId",
                table: "TripMembers",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_TripMembers_UserId",
                table: "TripMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_OrganizerId",
                table: "Trips",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Trips_TripId",
                table: "Expenses",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
