// Models/Solicitud.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSolicitudes.Models
{
    public class Solicitud
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Folio { get; set; } = string.Empty;

        [StringLength(200)]
        public string? NombreSolicitante { get; set; }

        [StringLength(200)]
        public string? Departamento { get; set; }

        [StringLength(500)]
        public string? RutaOficio { get; set; }

        [StringLength(500)]
        public string? RutaAnexos { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(10)]
        public string Estado { get; set; } = "activo";

        // Relación con TipoSolicitud
        public int TipoSolicitudID { get; set; }
        public TipoSolicitud? TipoSolicitud { get; set; }
    }
}