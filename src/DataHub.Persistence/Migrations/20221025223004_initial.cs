using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataHub.Persistence.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "energy_report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energy_report", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "region_energy",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    region = table.Column<string>(type: "text", nullable: false),
                    energy = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region_energy", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_energy_report_hash",
                table: "energy_report",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_region_energy_energy",
                table: "region_energy",
                column: "region",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energy_report");

            migrationBuilder.DropTable(
                name: "region_energy");
        }
    }
}
