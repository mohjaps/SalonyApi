using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class addVacation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "timeFormEvening",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "timeToEvening",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Vacations",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(nullable: false),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacations_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacations_UserID",
                schema: "ReqqaDBUser",
                table: "Vacations",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vacations",
                schema: "ReqqaDBUser");

            migrationBuilder.DropColumn(
                name: "timeFormEvening",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData");

            migrationBuilder.DropColumn(
                name: "timeToEvening",
                schema: "ReqqaDBUser",
                table: "ProviderAditionalData");
        }
    }
}
