using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSolicitudes.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Solicitudes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Solicitudes");
        }
    }
}
