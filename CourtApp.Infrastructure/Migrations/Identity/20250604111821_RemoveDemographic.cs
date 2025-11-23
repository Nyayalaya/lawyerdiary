using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtApp.Infrastructure.Migrations.Identity
{
    /// <inheritdoc />
    public partial class RemoveDemographic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_demographic_DemographicId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DemographicId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DemographicId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "demographic",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_demographic_UserId",
                schema: "Identity",
                table: "demographic",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_demographic_Users_UserId",
                schema: "Identity",
                table: "demographic",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_demographic_Users_UserId",
                schema: "Identity",
                table: "demographic");

            migrationBuilder.DropIndex(
                name: "IX_demographic_UserId",
                schema: "Identity",
                table: "demographic");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Identity",
                table: "demographic");

            migrationBuilder.AddColumn<Guid>(
                name: "DemographicId",
                schema: "Identity",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DemographicId",
                schema: "Identity",
                table: "Users",
                column: "DemographicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_demographic_DemographicId",
                schema: "Identity",
                table: "Users",
                column: "DemographicId",
                principalSchema: "Identity",
                principalTable: "demographic",
                principalColumn: "Id");
        }
    }
}
