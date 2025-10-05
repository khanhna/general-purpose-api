using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPurpose.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeAndIsActiveImageConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageVintageProcessConfigs_WorkingUnitId",
                table: "ImageVintageProcessConfigs");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ImageVintageProcessConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedTime",
                table: "ImageVintageProcessConfigs",
                type: "timestamp(0) with time zone",
                precision: 0,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedTime",
                table: "ImageCompositionConfigs",
                type: "timestamp(0) with time zone",
                precision: 0,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ImageVintageProcessConfigs_WorkingUnitId",
                table: "ImageVintageProcessConfigs",
                column: "WorkingUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageVintageProcessConfigs_WorkingUnitId",
                table: "ImageVintageProcessConfigs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ImageVintageProcessConfigs");

            migrationBuilder.DropColumn(
                name: "LastUpdatedTime",
                table: "ImageVintageProcessConfigs");

            migrationBuilder.DropColumn(
                name: "LastUpdatedTime",
                table: "ImageCompositionConfigs");

            migrationBuilder.CreateIndex(
                name: "IX_ImageVintageProcessConfigs_WorkingUnitId",
                table: "ImageVintageProcessConfigs",
                column: "WorkingUnitId",
                unique: true);
        }
    }
}
