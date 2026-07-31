using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GYMMANAGEMENTSYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkDateToTrainerSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkDate",
                table: "TrainerSchedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "TrainerSchedules");
        }
    }
}
