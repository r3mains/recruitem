using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFullRecruitmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    closed_reason = table.Column<string>(type: "text", nullable: true),
                    number_of_interviews = table.Column<int>(type: "integer", nullable: false),
                    reviewer_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.id);
                    table.ForeignKey(
                        name: "FK_positions_employees_reviewer_id",
                        column: x => x.reviewer_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_positions_status_types_status_id",
                        column: x => x.status_id,
                        principalTable: "status_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    country_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_states", x => x.id);
                    table.ForeignKey(
                        name: "FK_states_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "candidate_skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    candidate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    year_of_experience = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidate_skills", x => x.id);
                    table.ForeignKey(
                        name: "FK_candidate_skills_candidates_candidate_id",
                        column: x => x.candidate_id,
                        principalTable: "candidates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_candidate_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_skills", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_skills_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_job_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "position_skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    position_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position_skills", x => x.id);
                    table.ForeignKey(
                        name: "FK_position_skills_positions_position_id",
                        column: x => x.position_id,
                        principalTable: "positions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_position_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    state_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                    table.ForeignKey(
                        name: "FK_cities_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_line_1 = table.Column<string>(type: "text", nullable: true),
                    address_line_2 = table.Column<string>(type: "text", nullable: true),
                    locality = table.Column<string>(type: "text", nullable: true),
                    pincode = table.Column<string>(type: "text", nullable: true),
                    city_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_addresses_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobs_job_type_id",
                table: "jobs",
                column: "job_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_location",
                table: "jobs",
                column: "location");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_position_id",
                table: "jobs",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_recruiter_id",
                table: "jobs",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_status_id",
                table: "jobs",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_employees_branch_address",
                table: "employees",
                column: "branch_address");

            migrationBuilder.CreateIndex(
                name: "IX_candidates_address",
                table: "candidates",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "IX_addresses_city_id",
                table: "addresses",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_candidate_skills_candidate_id",
                table: "candidate_skills",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_candidate_skills_skill_id",
                table: "candidate_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_cities_state_id",
                table: "cities",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_skills_job_id",
                table: "job_skills",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_skills_skill_id",
                table: "job_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_position_skills_position_id",
                table: "position_skills",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_position_skills_skill_id",
                table: "position_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_positions_reviewer_id",
                table: "positions",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "IX_positions_status_id",
                table: "positions",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_states_country_id",
                table: "states",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "FK_candidates_addresses_address",
                table: "candidates",
                column: "address",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_addresses_branch_address",
                table: "employees",
                column: "branch_address",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_addresses_location",
                table: "jobs",
                column: "location",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_employees_recruiter_id",
                table: "jobs",
                column: "recruiter_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_job_types_job_type_id",
                table: "jobs",
                column: "job_type_id",
                principalTable: "job_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_positions_position_id",
                table: "jobs",
                column: "position_id",
                principalTable: "positions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_jobs_status_types_status_id",
                table: "jobs",
                column: "status_id",
                principalTable: "status_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_candidates_addresses_address",
                table: "candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_addresses_branch_address",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_jobs_addresses_location",
                table: "jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_jobs_employees_recruiter_id",
                table: "jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_jobs_job_types_job_type_id",
                table: "jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_jobs_positions_position_id",
                table: "jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_jobs_status_types_status_id",
                table: "jobs");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "candidate_skills");

            migrationBuilder.DropTable(
                name: "job_skills");

            migrationBuilder.DropTable(
                name: "position_skills");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "states");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropIndex(
                name: "IX_jobs_job_type_id",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_location",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_position_id",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_recruiter_id",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_jobs_status_id",
                table: "jobs");

            migrationBuilder.DropIndex(
                name: "IX_employees_branch_address",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_candidates_address",
                table: "candidates");
        }
    }
}
