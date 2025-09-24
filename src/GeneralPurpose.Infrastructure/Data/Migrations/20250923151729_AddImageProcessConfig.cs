using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GeneralPurpose.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageProcessConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FaceSlimmingEnabled",
                table: "WorkingUnits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SkinRetouchEnabled",
                table: "WorkingUnits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VintageProcessEnabled",
                table: "WorkingUnits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ImageCompositionConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppSystemId = table.Column<int>(type: "integer", nullable: true),
                    FileName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BlendMode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Threshold = table.Column<int>(type: "integer", nullable: false),
                    Feather = table.Column<int>(type: "integer", nullable: false),
                    Opacity = table.Column<decimal>(type: "numeric", nullable: false),
                    InvertThreshold = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCompositionConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageCompositionConfigs_AppSystems_AppSystemId",
                        column: x => x.AppSystemId,
                        principalTable: "AppSystems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImageVintageProcessConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkingUnitId = table.Column<int>(type: "integer", nullable: true),
                    Contrast = table.Column<decimal>(type: "numeric", nullable: false),
                    Grain = table.Column<decimal>(type: "numeric", nullable: false),
                    Vignette = table.Column<decimal>(type: "numeric", nullable: false),
                    Fade = table.Column<decimal>(type: "numeric", nullable: false),
                    TintIntensity = table.Column<decimal>(type: "numeric", nullable: false),
                    Dust = table.Column<decimal>(type: "numeric", nullable: false),
                    Scratches = table.Column<int>(type: "integer", nullable: false),
                    Hairs = table.Column<int>(type: "integer", nullable: false),
                    Blur = table.Column<decimal>(type: "numeric", nullable: false),
                    RedAdjustment = table.Column<int>(type: "integer", nullable: false),
                    GreenAdjustment = table.Column<int>(type: "integer", nullable: false),
                    BlueAdjustment = table.Column<int>(type: "integer", nullable: false),
                    Brightness = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageVintageProcessConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageVintageProcessConfigs_WorkingUnits_WorkingUnitId",
                        column: x => x.WorkingUnitId,
                        principalTable: "WorkingUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageCompositionConfigs_AppSystemId",
                table: "ImageCompositionConfigs",
                column: "AppSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageVintageProcessConfigs_WorkingUnitId",
                table: "ImageVintageProcessConfigs",
                column: "WorkingUnitId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageCompositionConfigs");

            migrationBuilder.DropTable(
                name: "ImageVintageProcessConfigs");

            migrationBuilder.DropColumn(
                name: "FaceSlimmingEnabled",
                table: "WorkingUnits");

            migrationBuilder.DropColumn(
                name: "SkinRetouchEnabled",
                table: "WorkingUnits");

            migrationBuilder.DropColumn(
                name: "VintageProcessEnabled",
                table: "WorkingUnits");
        }
    }
}
