using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSolicitudes.Migrations
{
    /// <inheritdoc />
    public partial class CorregirNombreEntidadSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoSolicitud",
                table: "Solicitudes");

            migrationBuilder.AddColumn<int>(
                name: "TipoSolicitudId",
                table: "Solicitudes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TiposSolicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSolicitud", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_TipoSolicitudId",
                table: "Solicitudes",
                column: "TipoSolicitudId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_TiposSolicitud_TipoSolicitudId",
                table: "Solicitudes",
                column: "TipoSolicitudId",
                principalTable: "TiposSolicitud",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_TiposSolicitud_TipoSolicitudId",
                table: "Solicitudes");

            migrationBuilder.DropTable(
                name: "TiposSolicitud");

            migrationBuilder.DropIndex(
                name: "IX_Solicitudes_TipoSolicitudId",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "TipoSolicitudId",
                table: "Solicitudes");

            migrationBuilder.AddColumn<string>(
                name: "TipoSolicitud",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
