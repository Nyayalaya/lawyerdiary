using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtApp.Infrastructure.Migrations.App
{
    /// <inheritdoc />
    public partial class AddFormID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormId",
                table: "case_petition_detail",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_case_petition_detail_FormId",
                table: "case_petition_detail",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_case_petition_detail_m_frm_types_FormId",
                table: "case_petition_detail",
                column: "FormId",
                principalTable: "m_frm_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_petition_detail_m_frm_types_FormId",
                table: "case_petition_detail");

            migrationBuilder.DropIndex(
                name: "IX_case_petition_detail_FormId",
                table: "case_petition_detail");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "case_petition_detail");
        }
    }
}
