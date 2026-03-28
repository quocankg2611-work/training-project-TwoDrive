using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwoDrive.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Files_OwnerId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Folders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserNameSnapshot",
                table: "Folders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Folders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserNameSnapshot",
                table: "Folders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Files",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserNameSnapshot",
                table: "Files",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Files",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserNameSnapshot",
                table: "Files",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_CreatedByUserId",
                table: "Folders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CreatedByUserId",
                table: "Files",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_CreatedByUserId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Files_CreatedByUserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserNameSnapshot",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserNameSnapshot",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CreatedByUserNameSnapshot",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserNameSnapshot",
                table: "Files");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Folders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Files",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_OwnerId",
                table: "Files",
                column: "OwnerId");
        }
    }
}
