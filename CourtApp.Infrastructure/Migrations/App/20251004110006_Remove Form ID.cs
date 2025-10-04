using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtApp.Infrastructure.Migrations.App
{
    /// <inheritdoc />
    public partial class RemoveFormID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_petition_detail_m_frm_types_DraftingFormId",
                table: "case_petition_detail");

            migrationBuilder.DropIndex(
                name: "IX_case_petition_detail_DraftingFormId",
                table: "case_petition_detail");

            migrationBuilder.DropColumn(
                name: "DraftingFormId",
                table: "case_petition_detail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DraftingFormId",
                table: "case_petition_detail",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_case_petition_detail_DraftingFormId",
                table: "case_petition_detail",
                column: "DraftingFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_case_petition_detail_m_frm_types_DraftingFormId",
                table: "case_petition_detail",
                column: "DraftingFormId",
                principalTable: "m_frm_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
