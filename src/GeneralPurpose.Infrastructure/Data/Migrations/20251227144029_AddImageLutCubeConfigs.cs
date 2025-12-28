using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GeneralPurpose.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageLutCubeConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ImageCompositionConfigs",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ImageLutCubeSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppSystemId = table.Column<int>(type: "integer", nullable: true),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    FileName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdatedTime = table.Column<DateTime>(type: "timestamp(0) with time zone", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageLutCubeSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageLutCubeSettings_AppSystems_AppSystemId",
                        column: x => x.AppSystemId,
                        principalTable: "AppSystems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageCompositionConfigs_FileName",
                table: "ImageCompositionConfigs",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageLutCubeSettings_AppSystemId",
                table: "ImageLutCubeSettings",
                column: "AppSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageLutCubeSettings_Code",
                table: "ImageLutCubeSettings",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_ImageLutCubeSettings_FileName",
                table: "ImageLutCubeSettings",
                column: "FileName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageLutCubeSettings");

            migrationBuilder.DropIndex(
                name: "IX_ImageCompositionConfigs_FileName",
                table: "ImageCompositionConfigs");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "ImageCompositionConfigs");
        }
    }
}
