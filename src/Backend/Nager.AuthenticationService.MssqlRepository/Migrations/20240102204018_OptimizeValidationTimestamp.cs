using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nager.AuthenticationService.MssqlRepository.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeValidationTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastValidationTimestamp",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSuccessfulValidationTimestamp",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFailedValidationTimestamp",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE Users SET LastSuccessfulValidationTimestamp = NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastFailedValidationTimestamp",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSuccessfulValidationTimestamp",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastValidationTimestamp",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
