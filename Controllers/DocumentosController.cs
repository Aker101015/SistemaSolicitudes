using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSolicitudes.Data;
using SistemaSolicitudes.Models;

namespace SistemaSolicitudes.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Subir() => View();

        [HttpPost]
        public async Task<IActionResult> Subir(
            string? folioSolicitud,
            string? folioRespuesta,
            IFormFile? oficioSolicitud,
            IFormFile? oficioRespuesta,
            IFormFile? anexos)
        {
            if (string.IsNullOrWhiteSpace(folioSolicitud) && string.IsNullOrWhiteSpace(folioRespuesta))
            {
                ModelState.AddModelError("", "Debe proporcionar Folio de Solicitud o Folio de Respuesta.");
                return View();
            }

            if (anexos != null && string.IsNullOrWhiteSpace(folioRespuesta))
            {
                ModelState.AddModelError("", "Para subir anexos, debe especificar el Folio de Respuesta.");
                return View();
            }

            string uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsRoot);

            string carpetaSolicitudes = Path.Combine(uploadsRoot, "Solicitudes");
            string carpetaRespuestas = Path.Combine(uploadsRoot, "Respuestas");
            string carpetaAnexos = Path.Combine(uploadsRoot, "Anexos");

            Directory.CreateDirectory(carpetaSolicitudes);
            Directory.CreateDirectory(carpetaRespuestas);
            Directory.CreateDirectory(carpetaAnexos);

            string? GuardarArchivo(IFormFile? archivo, string nombreBase, string carpeta)
            {
                if (archivo == null || archivo.Length == 0) return null;

                if (archivo.ContentType != "application/pdf" &&
                    !archivo.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"El archivo {archivo.FileName} no es un PDF válido.");
                }

                string nombre = $"{nombreBase}.pdf";
                string rutaFisica = Path.Combine(carpeta, nombre);

                if (System.IO.File.Exists(rutaFisica))
                {
                    throw new InvalidOperationException($"El archivo {nombre} ya existe. Use un folio único.");
                }

                using var stream = new FileStream(rutaFisica, FileMode.Create);
                archivo.CopyTo(stream);

                return $"/uploads/{Path.GetFileName(carpeta)}/{nombre}";
            }

            try
            {
                string? rutaSolicitud = null;
                string? rutaRespuesta = null;
                string? rutaAnexos = null;

                if (oficioSolicitud != null && !string.IsNullOrWhiteSpace(folioSolicitud))
                    rutaSolicitud = GuardarArchivo(oficioSolicitud, $"OF-{folioSolicitud.Trim()}", carpetaSolicitudes);

                if (oficioRespuesta != null && !string.IsNullOrWhiteSpace(folioRespuesta))
                    rutaRespuesta = GuardarArchivo(oficioRespuesta, $"OF-{folioRespuesta.Trim()}", carpetaRespuestas);

                if (anexos != null && !string.IsNullOrWhiteSpace(folioRespuesta))
                    rutaAnexos = GuardarArchivo(anexos, $"ANEXOS_{folioRespuesta.Trim()}", carpetaAnexos);

                var documento = new DocumentoOficio
                {
                    FolioSolicitud = folioSolicitud?.Trim(),
                    FolioRespuesta = folioRespuesta?.Trim(),
                    RutaSolicitud = rutaSolicitud,
                    RutaRespuesta = rutaRespuesta,
                    RutaAnexos = rutaAnexos,
                    Estado = "activo",
                    FechaRegistro = DateTime.UtcNow
                };

                _context.DocumentosOficio.Add(documento);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Documentos registrados correctamente.";
                return RedirectToAction(nameof(Subir));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View();
            }
        }

        public IActionResult Buscar() => View();

        [HttpPost]
        public async Task<IActionResult> Buscar(string? folioBusqueda)
        {
            ViewBag.Resultados = new List<DocumentoOficio>();
            if (!string.IsNullOrWhiteSpace(folioBusqueda))
            {
                ViewBag.Resultados = await _context.DocumentosOficio
                    .Where(d => d.Estado == "activo" &&
                               (!string.IsNullOrEmpty(d.FolioSolicitud) && d.FolioSolicitud.Contains(folioBusqueda) ||
                                !string.IsNullOrEmpty(d.FolioRespuesta) && d.FolioRespuesta.Contains(folioBusqueda)))
                    .OrderBy(d => d.ID)
                    .ToListAsync();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Inactivar(int id)
        {
            var doc = await _context.DocumentosOficio.FindAsync(id);
            if (doc != null)
            {
                doc.Estado = "inactivo";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Buscar));
        }


        [HttpGet]
        public async Task<IActionResult> ExportarExcel(string? folioBusqueda)
        {
            var documentos = await _context.DocumentosOficio
                .Where(d => d.Estado == "activo" &&
                           (!string.IsNullOrEmpty(d.FolioSolicitud) && d.FolioSolicitud.Contains(folioBusqueda) ||
                            !string.IsNullOrEmpty(d.FolioRespuesta) && d.FolioRespuesta.Contains(folioBusqueda)))
                .OrderBy(d => d.ID)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Documentos");

            // Encabezados
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Folio Solicitud";
            worksheet.Cell(1, 3).Value = "Folio Respuesta";
            worksheet.Cell(1, 4).Value = "Ruta Solicitud";
            worksheet.Cell(1, 5).Value = "Ruta Respuesta";
            worksheet.Cell(1, 6).Value = "Ruta Anexos";
            worksheet.Cell(1, 7).Value = "Estado";
            worksheet.Cell(1, 8).Value = "Fecha Registro";

            int row = 2;
            foreach (var doc in documentos)
            {
                worksheet.Cell(row, 1).Value = doc.ID;
                worksheet.Cell(row, 2).Value = doc.FolioSolicitud;
                worksheet.Cell(row, 3).Value = doc.FolioRespuesta;
                worksheet.Cell(row, 4).Value = doc.RutaSolicitud;
                worksheet.Cell(row, 5).Value = doc.RutaRespuesta;
                worksheet.Cell(row, 6).Value = doc.RutaAnexos;
                worksheet.Cell(row, 7).Value = doc.Estado;
                worksheet.Cell(row, 8).Value = doc.FechaRegistro;

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Documentos_{DateTime.Now:yyyyMMdd}.xlsx");
        }


    }
}