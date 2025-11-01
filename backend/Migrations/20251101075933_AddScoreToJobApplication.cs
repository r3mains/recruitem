using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreToJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cover_letter",
                table: "job_applications",
                newName: "resume_url");

            migrationBuilder.AddColumn<bool>(
                name: "is_required",
                table: "position_skills",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "min_years_experience",
                table: "position_skills",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "min_years_experience",
                table: "job_skills",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "job_applications",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_letter_url",
                table: "job_applications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated",
                table: "job_applications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "qualifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    qualification = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qualifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "candidate_qualifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    candidate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    qualification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    year_completed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    grade = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidate_qualifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_candidate_qualifications_candidates_candidate_id",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_candidate_qualifications_qualifications_qualification_id",
                        column: x => x.qualification_id,
                        principalTable: "qualifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_applications_status_id",
                table: "job_applications",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_candidate_qualifications_candidate_id",
                table: "candidate_qualifications",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_candidate_qualifications_qualification_id",
                table: "candidate_qualifications",
                column: "qualification_id");

            migrationBuilder.AddForeignKey(
                name: "FK_job_applications_status_types_status_id",
                table: "job_applications",
                column: "status_id",
                principalTable: "status_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_applications_status_types_status_id",
                table: "job_applications");

            migrationBuilder.DropTable(
                name: "candidate_qualifications");

            migrationBuilder.DropTable(
                name: "qualifications");

            migrationBuilder.DropIndex(
                name: "IX_job_applications_status_id",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "is_required",
                table: "position_skills");

            migrationBuilder.DropColumn(
                name: "min_years_experience",
                table: "position_skills");

            migrationBuilder.DropColumn(
                name: "min_years_experience",
                table: "job_skills");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "cover_letter_url",
                table: "job_applications");

            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "job_applications");

            migrationBuilder.RenameColumn(
                name: "resume_url",
                table: "job_applications",
                newName: "cover_letter");
        }
    }
}
