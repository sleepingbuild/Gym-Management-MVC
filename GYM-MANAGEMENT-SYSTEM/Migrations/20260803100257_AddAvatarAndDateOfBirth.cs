using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GYMMANAGEMENTSYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarAndDateOfBirth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "UserProfiles",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "Trainers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Trainers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Trainers");
        }
    }
}
