using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPositionModelsWithStatusSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventCandidateStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCandidateStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QualificationName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StateName = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CityName = table.Column<string>(type: "text", nullable: false),
                    StateId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    Pincode = table.Column<string>(type: "text", nullable: true),
                    CityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    ContactNumber = table.Column<string>(type: "text", nullable: true),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidates_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Candidates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    BranchAddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    JoiningDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OfferLetterUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Addresses_BranchAddressId",
                        column: x => x.BranchAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosedReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NumberOfInterviews = table.Column<int>(type: "integer", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Employees_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Positions_PositionStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PositionStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecruiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    JobTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalaryMin = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    SalaryMax = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosedReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.CheckConstraint("CK_Job_Salary", "\"SalaryMin\" IS NULL OR \"SalaryMax\" IS NULL OR \"SalaryMin\" <= \"SalaryMax\"");
                    table.ForeignKey(
                        name: "FK_Jobs_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_Employees_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_JobStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "JobStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_JobTypes_JobTypeId",
                        column: x => x.JobTypeId,
                        principalTable: "JobTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PositionSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionSkills_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobQualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    QualificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinRequired = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobQualifications_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobQualifications_Qualifications_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "Qualifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSkills_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApplicationStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "Applied" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "Screening" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "Shortlisted" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "Interview" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "Selected" },
                    { new Guid("40000000-0000-0000-0000-000000000006"), "Rejected" },
                    { new Guid("40000000-0000-0000-0000-000000000007"), "On Hold" }
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), "Resume" },
                    { new Guid("90000000-0000-0000-0000-000000000002"), "Cover Letter" },
                    { new Guid("90000000-0000-0000-0000-000000000003"), "Transcript" },
                    { new Guid("90000000-0000-0000-0000-000000000004"), "Certificate" },
                    { new Guid("90000000-0000-0000-0000-000000000005"), "Identity Proof" },
                    { new Guid("90000000-0000-0000-0000-000000000006"), "Address Proof" },
                    { new Guid("90000000-0000-0000-0000-000000000007"), "Portfolio" },
                    { new Guid("90000000-0000-0000-0000-000000000008"), "References" }
                });

            migrationBuilder.InsertData(
                table: "EventCandidateStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000001"), "Registered" },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "Checked-In" },
                    { new Guid("70000000-0000-0000-0000-000000000003"), "Interviewed" },
                    { new Guid("70000000-0000-0000-0000-000000000004"), "Selected" },
                    { new Guid("70000000-0000-0000-0000-000000000005"), "Rejected" }
                });

            migrationBuilder.InsertData(
                table: "InterviewStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "Planned" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "In Progress" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "Completed" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "JobStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Open" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "On Hold" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Closed" }
                });

            migrationBuilder.InsertData(
                table: "JobTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "Internship" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Full time" }
                });

            migrationBuilder.InsertData(
                table: "PositionStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Open" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "On Hold" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Closed" }
                });

            migrationBuilder.InsertData(
                table: "ScheduleStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), "Scheduled" },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "Rescheduled" },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "Completed" },
                    { new Guid("60000000-0000-0000-0000-000000000004"), "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "VerificationStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { new Guid("80000000-0000-0000-0000-000000000001"), "Pending" },
                    { new Guid("80000000-0000-0000-0000-000000000002"), "In Progress" },
                    { new Guid("80000000-0000-0000-0000-000000000003"), "Verified" },
                    { new Guid("80000000-0000-0000-0000-000000000004"), "Rejected" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_AddressId",
                table: "Candidates",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_UserId",
                table: "Candidates",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchAddressId",
                table: "Employees",
                column: "BranchAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCandidateStatuses_Status",
                table: "EventCandidateStatuses",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobQualifications_JobId",
                table: "JobQualifications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobQualifications_QualificationId",
                table: "JobQualifications",
                column: "QualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AddressId",
                table: "Jobs",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobTypeId",
                table: "Jobs",
                column: "JobTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PositionId",
                table: "Jobs",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_RecruiterId",
                table: "Jobs",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_StatusId",
                table: "Jobs",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_JobId",
                table: "JobSkills",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_SkillId",
                table: "JobSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ReviewerId",
                table: "Positions",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_StatusId",
                table: "Positions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionSkills_PositionId_SkillId",
                table: "PositionSkills",
                columns: new[] { "PositionId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionSkills_SkillId",
                table: "PositionSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationStatuses");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "EventCandidateStatuses");

            migrationBuilder.DropTable(
                name: "InterviewStatuses");

            migrationBuilder.DropTable(
                name: "JobQualifications");

            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "PositionSkills");

            migrationBuilder.DropTable(
                name: "ScheduleStatuses");

            migrationBuilder.DropTable(
                name: "VerificationStatuses");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "JobStatuses");

            migrationBuilder.DropTable(
                name: "JobTypes");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "PositionStatuses");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
