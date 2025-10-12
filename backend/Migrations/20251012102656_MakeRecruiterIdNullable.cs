using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
  /// <inheritdoc />
  public partial class MakeRecruiterIdNullable : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<Guid?>(
          name: "recruiter_id",
          table: "jobs",
          type: "uuid",
          nullable: true,
          oldClrType: typeof(Guid),
          oldType: "uuid");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<Guid>(
          name: "recruiter_id",
          table: "jobs",
          type: "uuid",
          nullable: false,
          oldClrType: typeof(Guid?),
          oldType: "uuid",
          oldNullable: true);
    }
  }
}
