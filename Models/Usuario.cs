using System.ComponentModel.DataAnnotations;

namespace SistemaSolicitudes.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; } = string.Empty;

        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        public string? CorreoElectronico { get; set; }

        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Usuario"; // Ej: "Admin", "Usuario"

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}