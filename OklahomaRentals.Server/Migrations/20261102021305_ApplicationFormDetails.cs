using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OklahomaRentals.Server.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationFormDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Bankruptcy",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BrokenLease",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "College",
                table: "Applications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Convicted",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentResidence_Address",
                table: "Applications",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentResidence_From",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentResidence_Landlord",
                table: "Applications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CurrentResidence_LandlordContact",
                table: "Applications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CurrentResidence_ReasonForLeaving",
                table: "Applications",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentResidence_To",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriversLicense",
                table: "Applications",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Evicted",
                table: "Applications",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedGraduation",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "Applications",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LeaseTerm",
                table: "Applications",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Major",
                table: "Applications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PreviousResidence_Address",
                table: "Applications",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "PreviousResidence_From",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousResidence_Landlord",
                table: "Applications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PreviousResidence_LandlordContact",
                table: "Applications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PreviousResidence_ReasonForLeaving",
                table: "Applications",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "PreviousResidence_To",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bankruptcy",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "BrokenLease",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "College",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Convicted",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CurrentResidence_Address",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CurrentResidence_From",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CurrentResidence_Landlord",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CurrentResidence_LandlordContact",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CurrentResidence_ReasonForLeaving",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CurrentResidence_To",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "DriversLicense",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Evicted",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ExpectedGraduation",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "LeaseTerm",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Major",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreviousResidence_Address",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreviousResidence_From",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreviousResidence_Landlord",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreviousResidence_LandlordContact",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreviousResidence_ReasonForLeaving",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreviousResidence_To",
                table: "Applications");
        }
    }
}
