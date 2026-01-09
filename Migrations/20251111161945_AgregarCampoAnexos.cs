using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSolicitudes.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoAnexos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoAnexos",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivoAnexos",
                table: "Solicitudes");
        }
    }
}
