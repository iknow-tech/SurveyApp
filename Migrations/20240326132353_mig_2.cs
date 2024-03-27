using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyApp.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyId = table.Column<int>(type: "INTEGER", nullable: true),
                    FacilityId = table.Column<int>(type: "INTEGER", nullable: true),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyStatuses_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SurveyStatuses_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SurveyStatuses_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyStatuses_AppUserId",
                table: "SurveyStatuses",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyStatuses_FacilityId",
                table: "SurveyStatuses",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyStatuses_SurveyId",
                table: "SurveyStatuses",
                column: "SurveyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyStatuses");
        }
    }
}
