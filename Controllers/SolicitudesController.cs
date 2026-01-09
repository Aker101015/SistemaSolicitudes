using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSolicitudes.Data;
using SistemaSolicitudes.Models;

namespace SistemaSolicitudes.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SolicitudesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Solicitudes
        public async Task<IActionResult> Index(string? folioBusqueda)
        {
            var query = _context.Solicitudes
                .Include(s => s.TipoSolicitud)
                .Where(s => s.Estado == "activo");

            if (!string.IsNullOrWhiteSpace(folioBusqueda))
            {
                query = query.Where(s =>
                    s.Folio.Contains(folioBusqueda) ||
                    s.NombreSolicitante!.Contains(folioBusqueda));
            }

            var solicitudes = await query.OrderBy(s => s.ID).ToListAsync();
            ViewBag.FolioBusqueda = folioBusqueda;
            return View(solicitudes);
        }

        // GET: Solicitudes/Create
        public IActionResult Create()
        {
            ViewBag.TiposSolicitud = _context.TiposSolicitud.ToList();
            return View();
        }

        // POST: Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string folio,
            string? nombreSolicitante,
            int tipoSolicitudID,
            string? departamento,
            IFormFile? oficioSolicitud,
            IFormFile? oficioRespuesta,
            IFormFile? anexos)
        {
            try
            {
                // Validar folio
                if (string.IsNullOrWhiteSpace(folio))
                {
                    ModelState.AddModelError("", "El folio es obligatorio.");
                    ViewBag.TiposSolicitud = _context.TiposSolicitud.ToList();
                    return View();
                }

                // Validar tipo de solicitud
                if (!_context.TiposSolicitud.Any(t => t.ID == tipoSolicitudID))
                {
                    ModelState.AddModelError("", "Tipo de solicitud no válido.");
                    ViewBag.TiposSolicitud = _context.TiposSolicitud.ToList();
                    return View();
                }

                // Rutas de carpetas
                string uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                string solicitudesPath = Path.Combine(uploads, "Solicitudes");
                string respuestasPath = Path.Combine(uploads, "Respuestas");
                string anexosPath = Path.Combine(uploads, "Anexos");

                Directory.CreateDirectory(solicitudesPath);
                Directory.CreateDirectory(respuestasPath);
                Directory.CreateDirectory(anexosPath);

                // Función para guardar PDF
                string? GuardarPdf(IFormFile? archivo, string nombreBase, string carpeta)
                {
                    if (archivo == null || archivo.Length == 0) return null;

                    if (!archivo.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Solo se permiten archivos PDF.");

                    string nombre = $"{nombreBase}.pdf";
                    string rutaFisica = Path.Combine(carpeta, nombre);

                    if (System.IO.File.Exists(rutaFisica))
                        throw new InvalidOperationException($"El archivo {nombre} ya existe.");

                    using var stream = new FileStream(rutaFisica, FileMode.Create);
                    archivo.CopyTo(stream);

                    return $"/uploads/{Path.GetFileName(carpeta)}/{nombre}";
                }

                // Guardar archivos
                string? rutaOficioSolicitud = null;
                string? rutaOficioRespuesta = null;
                string? rutaAnexos = null;

                if (oficioSolicitud != null)
                    rutaOficioSolicitud = GuardarPdf(oficioSolicitud, $"OF-{folio.Trim()}", solicitudesPath);

                if (oficioRespuesta != null)
                    rutaOficioRespuesta = GuardarPdf(oficioRespuesta, $"OF-{folio.Trim()}", respuestasPath);

                if (anexos != null)
                    rutaAnexos = GuardarPdf(anexos, $"ANEXOS_{folio.Trim()}", anexosPath);

                // Crear solicitud
                var solicitud = new Solicitud
                {
                    Folio = folio.Trim(),
                    NombreSolicitante = nombreSolicitante?.Trim(),
                    Departamento = departamento?.Trim(),
                    TipoSolicitudID = tipoSolicitudID,
                    RutaOficio = rutaOficioSolicitud,
                    RutaAnexos = rutaAnexos,
                    Estado = "activo",
                    FechaRegistro = DateTime.UtcNow
                };

                _context.Solicitudes.Add(solicitud);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Solicitud registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                ViewBag.TiposSolicitud = _context.TiposSolicitud.ToList();
                return View();
            }
        }

        // POST: Solicitudes/Inactivar/5
        [HttpPost]
        public async Task<IActionResult> Inactivar(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud != null)
            {
                solicitud.Estado = "inactivo";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Solicitudes/Delete (solo marca como inactivo)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var solicitud = await _context.Solicitudes
                .Include(s => s.TipoSolicitud)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (solicitud == null) return NotFound();

            return View(solicitud);
        }

        // POST: Solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return await Inactivar(id);
        }


        // GET: Solicitudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound();

            ViewBag.TiposSolicitud = await _context.TiposSolicitud.ToListAsync();
            return View(solicitud);
        }

        // POST: Solicitudes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string folio,
            string? nombreSolicitante,
            int tipoSolicitudID,
            string? departamento,
            IFormFile? oficioSolicitud,
            IFormFile? anexos)
        {
            if (id <= 0) return NotFound();

            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound();

            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(folio))
                    throw new InvalidOperationException("El folio es obligatorio.");

                if (!_context.TiposSolicitud.Any(t => t.ID == tipoSolicitudID))
                    throw new InvalidOperationException("Tipo de solicitud no válido.");

                // Rutas
                string uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                string solicitudesPath = Path.Combine(uploads, "Solicitudes");
                string anexosPath = Path.Combine(uploads, "Anexos");

                Directory.CreateDirectory(solicitudesPath);
                Directory.CreateDirectory(anexosPath);

                // Función para guardar PDF (reutilizable)
                string? GuardarPdf(IFormFile? archivo, string nombreBase, string carpeta)
                {
                    if (archivo == null || archivo.Length == 0) return null;
                    if (!archivo.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Solo se permiten archivos PDF.");

                    string nombre = $"{nombreBase}.pdf";
                    string rutaFisica = Path.Combine(carpeta, nombre);

                    using var stream = new FileStream(rutaFisica, FileMode.Create);
                    archivo.CopyTo(stream);
                    return $"/uploads/{Path.GetFileName(carpeta)}/{nombre}";
                }

                // Guardar nuevos archivos (si se suben)
                if (oficioSolicitud != null)
                {
                    solicitud.RutaOficio = GuardarPdf(oficioSolicitud, $"OF-{folio.Trim()}", solicitudesPath);
                }

                if (anexos != null)
                {
                    solicitud.RutaAnexos = GuardarPdf(anexos, $"ANEXOS_{folio.Trim()}", anexosPath);
                }

                // Actualizar campos
                solicitud.Folio = folio.Trim();
                solicitud.NombreSolicitante = nombreSolicitante?.Trim();
                solicitud.Departamento = departamento?.Trim();
                solicitud.TipoSolicitudID = tipoSolicitudID;

                _context.Update(solicitud);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Solicitud actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                ViewBag.TiposSolicitud = await _context.TiposSolicitud.ToListAsync();
                return View(solicitud);
            }
        }
    }
}