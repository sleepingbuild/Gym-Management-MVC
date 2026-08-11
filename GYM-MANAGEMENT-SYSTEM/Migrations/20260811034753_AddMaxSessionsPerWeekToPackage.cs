using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GYMMANAGEMENTSYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxSessionsPerWeekToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxSessionsPerWeek",
                table: "MembershipPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastHeightM",
                table: "ChatSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastWeightKg",
                table: "ChatSessions",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSessionsPerWeek",
                table: "MembershipPackages");

            migrationBuilder.DropColumn(
                name: "LastHeightM",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "LastWeightKg",
                table: "ChatSessions");
        }
    }
}
