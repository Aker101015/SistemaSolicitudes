using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSolicitudes.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoProg = table.Column<int>(type: "int", nullable: false),
                    NoOficioSolicitud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreSolicitante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolicitudDetalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FolioRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NombreFirmante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreResponsableRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreRegistrador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArchivoPdfSolicitud = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArchivoPdfRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoSolicitud = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Solicitudes");
        }
    }
}
