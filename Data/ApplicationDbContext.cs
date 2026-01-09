// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using SistemaSolicitudes.Models;

namespace SistemaSolicitudes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<TipoSolicitud> TiposSolicitud { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sembrar datos iniciales
            modelBuilder.Entity<TipoSolicitud>().HasData(
                new TipoSolicitud { ID = 1, Nombre = "General", Descripcion = "Solicitud general" },
                new TipoSolicitud { ID = 2, Nombre = "Urgente", Descripcion = "Solicitud urgente" }
            );
        }
    }
}