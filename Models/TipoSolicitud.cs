// Models/TipoSolicitud.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSolicitudes.Models
{
    public class TipoSolicitud
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        // Relación inversa (opcional)
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    }
}