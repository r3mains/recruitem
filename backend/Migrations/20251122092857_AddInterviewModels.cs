using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviews_InterviewStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "InterviewStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interviews_InterviewTypes_InterviewTypeId",
                        column: x => x.InterviewTypeId,
                        principalTable: "InterviewTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interviews_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interviewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviewers_Employees_InterviewerId",
                        column: x => x.InterviewerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interviewers_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ForSkill = table.Column<Guid>(type: "uuid", nullable: false),
                    FeedbackBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewFeedbacks", x => x.Id);
                    table.CheckConstraint("CK_InterviewFeedback_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_Employees_FeedbackBy",
                        column: x => x.FeedbackBy,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_Skills_ForSkill",
                        column: x => x.ForSkill,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MeetingLink = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSchedules_Employees_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewSchedules_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewSchedules_ScheduleStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ScheduleStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "InterviewTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "Technical" },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "HR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interviewers_InterviewerId",
                table: "Interviewers",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviewers_InterviewId",
                table: "Interviewers",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_FeedbackBy",
                table: "InterviewFeedbacks",
                column: "FeedbackBy");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_ForSkill",
                table: "InterviewFeedbacks",
                column: "ForSkill");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_InterviewId",
                table: "InterviewFeedbacks",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewTypeId",
                table: "Interviews",
                column: "InterviewTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_JobApplicationId",
                table: "Interviews",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_StatusId",
                table: "Interviews",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_CreatedBy",
                table: "InterviewSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_InterviewId",
                table: "InterviewSchedules",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedules_StatusId",
                table: "InterviewSchedules",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interviewers");

            migrationBuilder.DropTable(
                name: "InterviewFeedbacks");

            migrationBuilder.DropTable(
                name: "InterviewSchedules");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "InterviewTypes");
        }
    }
}
