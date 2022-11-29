using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class addAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "WokerID",
            //    schema: "ReqqaDBUser",
            //    table: "SubServices",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "PointsBalance",
            //    schema: "ReqqaDBUser",
            //    table: "AspNetUsers",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Appointments",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SallonID = table.Column<string>(nullable: false),
                    UserID = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_SallonID",
                        column: x => x.SallonID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SallonEvaluations",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    SallonID = table.Column<string>(nullable: false),
                    UserID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SallonEvaluations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SallonEvaluations_AspNetUsers_SallonID",
                        column: x => x.SallonID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SallonEvaluations_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameAr = table.Column<string>(nullable: false),
                    nameEn = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: false),
                    AttendanceDays = table.Column<string>(nullable: false),
                    SallonID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Workers_AspNetUsers_SallonID",
                        column: x => x.SallonID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerEvaluations",
                schema: "ReqqaDBUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    WorkerID = table.Column<int>(nullable: false),
                    UserID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerEvaluations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WorkerEvaluations_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerEvaluations_Workers_WorkerID",
                        column: x => x.WorkerID,
                        principalSchema: "ReqqaDBUser",
                        principalTable: "Workers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubServices_WokerID",
                schema: "ReqqaDBUser",
                table: "SubServices",
                column: "WokerID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SallonID",
                schema: "ReqqaDBUser",
                table: "Appointments",
                column: "SallonID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserID",
                schema: "ReqqaDBUser",
                table: "Appointments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SallonEvaluations_SallonID",
                schema: "ReqqaDBUser",
                table: "SallonEvaluations",
                column: "SallonID");

            migrationBuilder.CreateIndex(
                name: "IX_SallonEvaluations_UserID",
                schema: "ReqqaDBUser",
                table: "SallonEvaluations",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEvaluations_UserID",
                schema: "ReqqaDBUser",
                table: "WorkerEvaluations",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEvaluations_WorkerID",
                schema: "ReqqaDBUser",
                table: "WorkerEvaluations",
                column: "WorkerID");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_SallonID",
                schema: "ReqqaDBUser",
                table: "Workers",
                column: "SallonID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubServices_Workers_WokerID",
                schema: "ReqqaDBUser",
                table: "SubServices",
                column: "WokerID",
                principalSchema: "ReqqaDBUser",
                principalTable: "Workers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubServices_Workers_WokerID",
                schema: "ReqqaDBUser",
                table: "SubServices");

            migrationBuilder.DropTable(
                name: "Appointments",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "SallonEvaluations",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "WorkerEvaluations",
                schema: "ReqqaDBUser");

            migrationBuilder.DropTable(
                name: "Workers",
                schema: "ReqqaDBUser");

            migrationBuilder.DropIndex(
                name: "IX_SubServices_WokerID",
                schema: "ReqqaDBUser",
                table: "SubServices");

            migrationBuilder.DropColumn(
                name: "WokerID",
                schema: "ReqqaDBUser",
                table: "SubServices");

            migrationBuilder.DropColumn(
                name: "PointsBalance",
                schema: "ReqqaDBUser",
                table: "AspNetUsers");
        }
    }
}
