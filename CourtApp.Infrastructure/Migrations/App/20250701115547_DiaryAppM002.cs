using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtApp.Infrastructure.Migrations.App
{
    /// <inheritdoc />
    public partial class DiaryAppM002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_detail_against_m_cadre_CadreId",
                schema: "ld",
                table: "case_detail_against");

            migrationBuilder.AlterColumn<Guid>(
                name: "CadreId",
                schema: "ld",
                table: "case_detail_against",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_case_detail_against_m_cadre_CadreId",
                schema: "ld",
                table: "case_detail_against",
                column: "CadreId",
                principalSchema: "ld",
                principalTable: "m_cadre",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_detail_against_m_cadre_CadreId",
                schema: "ld",
                table: "case_detail_against");

            migrationBuilder.AlterColumn<Guid>(
                name: "CadreId",
                schema: "ld",
                table: "case_detail_against",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_case_detail_against_m_cadre_CadreId",
                schema: "ld",
                table: "case_detail_against",
                column: "CadreId",
                principalSchema: "ld",
                principalTable: "m_cadre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
